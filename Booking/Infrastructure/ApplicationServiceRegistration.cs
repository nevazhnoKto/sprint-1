using Booking.Application.Interfaces;
using Booking.Infrastructure.DataAccess;
using Booking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IBookingRepository, BookingRepository>();

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
