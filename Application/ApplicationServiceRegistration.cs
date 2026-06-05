using Application.Interfaces;
using Application.Services;
using Application.Services.BackgroundServices;
using Application.Validators;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// Регистрация сервисов.
			services.AddScoped<IEventService, EventService>();
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
