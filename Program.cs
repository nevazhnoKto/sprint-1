using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;
using WebApiTamakulov;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Mappings;
using WebApiTamakulov.Services;
using WebApiTamakulov.Services.BackgroundServices;
using WebApiTamakulov.Validators;
using Mapster;
using MapsterMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
	// Путь к XML-файлу с документацией
	var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
	options.IncludeXmlComments(xmlPath);
});

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEventRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateEventRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GetEventsRequestDtoValidator>();
builder.Services.AddHostedService<ConfirmBookingBackgroundService>();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
