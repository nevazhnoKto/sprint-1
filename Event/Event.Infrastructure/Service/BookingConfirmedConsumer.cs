using Confluent.Kafka;
using Event.Application.Interfaces;
using Event.Infrastructure.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContract;
using System.Text.Json;

namespace Event.Infrastructure.Service
{
	public class BookingConfirmedConsumer : BackgroundService
	{
		private readonly ILogger<BookingConfirmedConsumer> _logger;
		private readonly IServiceProvider _serviceProvider;
		private readonly IConfiguration _configuration;
		public BookingConfirmedConsumer(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<BookingConfirmedConsumer> logger)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_configuration = configuration;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			// Избавляемся от Task.Run. Инициализируем поток стандартным способом.
			// Библиотека Confluent.Kafka сама отлично управляет блокировкой потока.
			return Task.Factory.StartNew(
				() => StartConsumerLoop(stoppingToken),
				stoppingToken,
				TaskCreationOptions.LongRunning, // Указывает .NET выделить отдельный выделенный поток
				TaskScheduler.Default);
		}

		private void StartConsumerLoop(CancellationToken stoppingToken)
		{
			var config = new ConsumerConfig
			{
				BootstrapServers = _configuration["Kafka:BootstrapServers"],
				GroupId = _configuration["Kafka:ConsumerGroup"],
				AutoOffsetReset = AutoOffsetReset.Earliest,
				EnableAutoCommit = false
			};

			using var consumer = new ConsumerBuilder<string, string>(config).Build();
			consumer.Subscribe(BookingTopics.BookingConfirmed);

			_logger.LogInformation("Подписчик Kafka запущен и слушает топик '{Topic}'...", BookingTopics.BookingConfirmed);

			try
			{
				while (!stoppingToken.IsCancellationRequested)
				{
					try
					{
						// Блокирующий вызов. Ждет новое сообщение из Kafka
						var consumeResult = consumer.Consume(stoppingToken);
						if (consumeResult == null) continue;

						using var scope = _serviceProvider.CreateScope();

						// Используем .GetAwaiter().GetResult(), чтобы метод оставался строго синхронным в этом потоке
						ProcessMessageAsync(consumeResult.Message.Value, scope).GetAwaiter().GetResult();

						consumer.Commit(consumeResult);
					}
					catch (OperationCanceledException)
					{
						// Сюда мы попадем при остановке приложения (когда сработает stoppingToken)
						break;
					}
					catch (ConsumeException ex)
					{
						_logger.LogError($"Ошибка при получении сообщения из Kafka: {ex.Error.Reason}");
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Ошибка во время обработки бизнес-логики сообщения.");

						// ИСПРАВЛЕНИЕ: Используем синхронную задержку, чтобы не терять контекст потока Kafka
						stoppingToken.WaitHandle.WaitOne(1000);
					}
				}
			}
			finally
			{
				consumer.Close();
			}
		}

		private async Task ProcessMessageAsync(string jsonMessage, IServiceScope scope)
		{
			BookingConfirmedEvent? message;
			try
			{
				message = JsonSerializer.Deserialize<BookingConfirmedEvent>(jsonMessage);
				if (message == null) return;
			}
			catch (JsonException ex)
			{
				_logger.LogError(ex, "Не удалось десериализовать сообщение: {Message}", jsonMessage);
				return;
			}

			// 1. Извлекаем контекст БД для управления транзакцией
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

			// 2. Открываем транзакцию
			using var transaction = await dbContext.Database.BeginTransactionAsync();

			try
			{
				var inboxMessageRepo = scope.ServiceProvider.GetRequiredService<IInboxRepository>();

				// 3. Проверяем и сохраняем в Inbox
				var isUnique = await inboxMessageRepo.TrySaveAsync(message.BookingId, nameof(BookingConfirmedEvent));

				if (isUnique)
				{
					var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();

					// 4. Списываем/резервируем места
					var resultReserve = await eventService.TryReserveSeats(message.EventId);

					// 5. Фиксируем транзакцию (только теперь данные запишутся в БД одновременно)
					await transaction.CommitAsync();

					if (resultReserve)
					{
						_logger.LogInformation("Успешно обработано подтверждение брони {BookingId} для события {EventId}", message.BookingId, message.EventId);
					}
					else
					{
						_logger.LogWarning("Не удалось зарезервировать места для брони {BookingId} для события {EventId}", message.BookingId, message.EventId);
					}
				}
				else
				{
					// Если это дубликат, просто закрываем транзакцию без коммита
					_logger.LogInformation("Сообщение для брони {BookingId} является дубликатом. Пропускаем.", message.BookingId);
				}
			}
			catch (Exception ex)
			{
				// 6. Если упал репозиторий, или упал сервис мест — откатываем ВСЁ назад.
				// Запись в Inbox сотрется, места не изменятся. Консьюмер попробует обработать сообщение еще раз.
				await transaction.RollbackAsync();

				_logger.LogError(ex, "Ошибка при обработке бизнес-логики для брони {BookingId} у события {EventId}", message.BookingId, message.EventId);

				// ОБЯЗАТЕЛЬНО прокидываем ошибку дальше! 
				// Если ее заглушить, консьюмер сделает Commit в Kafka, и сообщение уйдет из очереди навсегда, оставшись необработанным.
				throw;
			}
		}
	}
}
