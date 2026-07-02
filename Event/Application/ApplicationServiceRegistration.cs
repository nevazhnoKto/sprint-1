using Event.Application.Interfaces;
using Event.Application.Services;
using Event.Application.Validators;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Event.Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			// Регистрация сервисов.
			services.AddScoped<IEventService, EventService>();

			// Регистрация для мапстера.
			services.AddSingleton(TypeAdapterConfig.GlobalSettings);
			services.AddScoped<IMapper, ServiceMapper>();

			return services;
		}
	}
}
