using Application.Interfaces;
using Application.Services;
using Application.Services.BackgroundServices;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// Регистрация сервисов.
			services.AddScoped<IBookingService, BookingService>();

			// Регистрация для мапстера.
			services.AddSingleton(TypeAdapterConfig.GlobalSettings);
			services.AddScoped<IMapper, ServiceMapper>();

			// Регистрация фонового процесса.
			services.AddHostedService<ConfirmBookingBackgroundService>();
			return services;
		}
	}
}
