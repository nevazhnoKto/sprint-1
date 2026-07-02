using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SharedContract;

namespace Event.Infrastructure;

public sealed class KafkaInitializer : IHostedService
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<KafkaInitializer> _logger;

	public KafkaInitializer(IConfiguration configuration, ILogger<KafkaInitializer> _logger)
	{
		_configuration = configuration;
		this._logger = _logger;
	}

	public async Task StartAsync(CancellationToken cancellationToken)
	{
		var bootstrapServers = _configuration["Kafka:BootstrapServers"];
		var config = new AdminClientConfig { BootstrapServers = bootstrapServers };

		using var adminClient = new AdminClientBuilder(config).Build();

		try
		{
			_logger.LogInformation("Проверка и создание топика Kafka...");

			await adminClient.CreateTopicsAsync(new TopicSpecification[]
			{
				new()
				{
					Name = BookingTopics.BookingConfirmed,
					NumPartitions = 3,
                    ReplicationFactor = 1
                }
			});

			_logger.LogInformation("Топик '{Topic}' успешно создан.", BookingTopics.BookingConfirmed);
		}
		catch (CreateTopicsException e) when (e.Results[0].Error.Code == ErrorCode.TopicAlreadyExists)
		{
			_logger.LogInformation("Топик '{Topic}' уже существует. Пропускаем создание.", BookingTopics.BookingConfirmed);
		}
		catch (Exception ex)
		{
			// Оформляем лог согласно требованию "не валить запуск сервиса"
			_logger.LogError(ex, "Не удалось создать топик Kafka. Сервис продолжит запуск.");
		}
	}

	public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}