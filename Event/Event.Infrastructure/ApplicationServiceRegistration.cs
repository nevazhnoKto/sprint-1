using Event.Application.Interfaces;
using Event.Infrastructure.DataAccess;
using Event.Infrastructure.Models;
using Event.Infrastructure.Repositories;
using Event.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Event.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public async static Task<IServiceCollection> AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			// 1. Инициализатор топика (отработает самым первым)
			services.AddHostedService<KafkaInitializer>();

			// 2. Постоянный фоновый обработчик сообщений
			services.AddHostedService<BookingConfirmedConsumer>();

			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IInboxRepository, InboxRepository>();
			services.AddSingleton<IRedisService, RedisService>();

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException(
					"Connection string 'DefaultConnection' not found in appsettings.json or environment variables.");
			}

			// Подключение Redis с настройками из appsettings.
			var redisSection = configuration.GetSection("Redis");
			services.Configure<RedisSettings>(redisSection);

			var connectionStringRedis = redisSection.Get<RedisSettings>()?.ConnectionString;
			
			var options = ConfigurationOptions.Parse(connectionStringRedis!);

			services.AddSingleton<IConnectionMultiplexer>(await ConnectionMultiplexer.ConnectAsync(options));

			services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(connectionString),
				ServiceLifetime.Scoped);

			return services;
		}
	}
}
