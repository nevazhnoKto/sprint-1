using Event.Application.Interfaces;
using Event.Infrastructure.DataAccess;
using Event.Infrastructure.Repositories;
using Event.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Event.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			// 1. Инициализатор топика (отработает самым первым)
			services.AddHostedService<KafkaInitializer>();

			// 2. Постоянный фоновый обработчик сообщений
			services.AddHostedService<BookingConfirmedConsumer>();

			services.AddScoped<IEventRepository, EventRepository>();

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException(
					"Connection string 'DefaultConnection' not found in appsettings.json or environment variables.");
			}

			services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(connectionString),
				ServiceLifetime.Scoped);

			return services;
		}
	}
}
