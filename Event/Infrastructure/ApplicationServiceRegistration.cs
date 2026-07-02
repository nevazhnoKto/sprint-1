using Event.Application.Interfaces;
using Event.Infrastructure.DataAccess;
using Event.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Event.Infrastructure
{
	public static class InfrastructureServiceRegistration
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			
			services.AddScoped<IEventRepository, EventRepository>();
			/*services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IPasswordHasher, PasswordHasher>();
			services.AddScoped<ITokenGenerator, TokenGenerator>();
			services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

			var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

			services.AddAuthentication("Bearer")
					.AddJwtBearer(options =>
					{
						options.TokenValidationParameters = new TokenValidationParameters
						{
							RoleClaimType = ClaimTypes.Role,
							ValidateIssuer = true,
							ValidIssuer = jwtSettings!.Issuer,
							ValidateAudience = true,
							ValidAudience = jwtSettings.Audience,
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							IssuerSigningKey = new SymmetricSecurityKey(
								Encoding.UTF8.GetBytes(jwtSettings.SecretKey)
							),
							ClockSkew = TimeSpan.Zero
						};
	});*/

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
