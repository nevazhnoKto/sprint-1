
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using User.Application.Interfaces;
using User.Application.Services;

namespace User.Application
{
	public static class ApplicationServiceRegistration
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddScoped<IUserService, UserService>();

			// Регистрация для мапстера.
			services.AddSingleton(TypeAdapterConfig.GlobalSettings);
			services.AddScoped<IMapper, ServiceMapper>();

			return services;
		}
	}
}
