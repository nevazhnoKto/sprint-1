using Confluent.Kafka;
using Event.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContract;
using System.Text.Json;
using System.Threading;
using static Confluent.Kafka.ConfigPropertyNames;

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
			// Запускаем в отдельном потоке, так как метод Consume блокирующий
			return Task.Run(() => StartConsumerLoop(stoppingToken), stoppingToken);
		}

		private async Task StartConsumerLoop(CancellationToken stoppingToken)
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

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var consumeResult = consumer.Consume(stoppingToken);
					if (consumeResult == null) continue;

					using var scope = _serviceProvider.CreateScope();
					await ProcessMessageAsync(consumeResult.Message.Value, scope);

					consumer.StoreOffset(consumeResult);
					consumer.Commit();
				}
				catch (ConsumeException ex)
				{
					Console.WriteLine($"Ошибка при получении сообщения: {ex.Error.Reason}");
				}
				finally
				{
					consumer.Close();
				}
			}
		}

		private async Task ProcessMessageAsync(string jsonMessage, IServiceScope scope)
		{
			BookingConfirmedEvent message;
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

			try
			{
				var eventService = scope.ServiceProvider.GetRequiredService<IEventService>();
				await eventService.TryReserveSeats(message.EventId);
				_logger.LogInformation("Успешно обработано подтверждение брони {BookingId} для события {EventId}", message.BookingId, message.EventId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при обработке бизнес-логики для брони {BookingId}", message.BookingId);
			}
		}
	}
}
