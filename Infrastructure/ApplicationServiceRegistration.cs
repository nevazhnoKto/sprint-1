using Application.Interfaces;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;
using Infrastructure.SecurityServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddScoped<IBookingRepository, BookingRepository>();
			services.AddScoped<IEventRepository, EventRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.AddScoped<ITokenGenerator, TokenGenerator>();

			var connectionString = configuration.GetConnectionString("DefaultConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException(
					"Connection string 'DefaultConnection' not found in appsettings.json or environment variables.");
			}

			services.AddDbContext<AppDbContext>(options =>
				options.UseNpgsql(connectionString));

			return services;
		}
	}
}
