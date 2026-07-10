using Booking.Application.Interfaces;
using Booking.Application.Services;
using Booking.Application.Services.BackgroundServices;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Application
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
