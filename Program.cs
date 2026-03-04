using FluentValidation;
using FluentValidation.AspNetCore;
using System.Reflection;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Mappings;
using WebApiTamakulov.Services;
using WebApiTamakulov.Validators;

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
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MappingProfile>(); }, typeof(Program));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<EventDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
