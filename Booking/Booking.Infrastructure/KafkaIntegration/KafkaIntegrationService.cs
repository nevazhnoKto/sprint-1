using Booking.Application.Interfaces;
using Booking.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContract;
using System.Text.Json;

namespace Booking.Infrastructure.KafkaIntegration
{
	public class KafkaIntegrationService : IKafkaIntegration, IDisposable
	{
		private readonly ILogger<KafkaIntegrationService> _logger;
		private readonly IProducer<string, string> _producer;
		public KafkaIntegrationService(IConfiguration configuration, IHostApplicationLifetime appLifetime, ILogger<KafkaIntegrationService> logger)
		{
			_logger = logger;
			var bootstrapServers = configuration["Kafka:BootstrapServers"]
				?? throw new InvalidOperationException("Kafka BootstrapServers is not configured.");
			var config = new ProducerConfig
			{
				BootstrapServers = bootstrapServers,
				Acks = Acks.All
			};
			_producer = new ProducerBuilder<string, string>(config).Build();

			// Привязываем хук к моменту остановки приложения
			appLifetime.ApplicationStopping.Register(OnApplicationStopping);
		}
		public async Task SendBookingConfirmedKafka(BookingModel bookingModel)
		{
			var request = new BookingConfirmedEvent(bookingModel.Id, bookingModel.EventId, bookingModel.UserId, 1, DateTime.Now);
			var kafkaMessage = new Message<string, string>
			{
				Key = bookingModel.EventId.ToString(),
				Value = JsonSerializer.Serialize(request)
			};

			var result = await _producer.ProduceAsync(BookingTopics.BookingConfirmed, kafkaMessage);

			_logger.LogInformation($"Доставлено: {result.TopicPartitionOffset}");
		}
	
		public async Task SendBookingCanceledKafka(BookingModel bookingModel)
		{
			var request = new BookingCanceledEvent(bookingModel.Id, bookingModel.EventId, bookingModel.UserId, "Захотел");
			var kafkaMessage = new Message<string, string>
			{
				Key = bookingModel.EventId.ToString(),
				Value = JsonSerializer.Serialize(request)
			};

			var result = await _producer.ProduceAsync(BookingTopics.BookingCanceled, kafkaMessage);

			_logger.LogInformation($"Доставлено: {result.TopicPartitionOffset}");
		}

		private void OnApplicationStopping()
		{
			_logger.LogInformation("IHostApplicationLifetime: Вызывается Flush для продюсера Kafka...");
			_producer.Flush(TimeSpan.FromSeconds(5));
		}

		public void Dispose()
		{
			_logger.LogInformation("Освобождение ресурсов продюсера Kafka (Dispose).");
			_producer.Dispose();
		}
	}
}
