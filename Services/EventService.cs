using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Linq.Expressions;
using WebApiTamakulov.Controllers;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
	public class EventService : IEventService
	{
		private static List<Event> Events { get; set; } =
			[
				new Event
				{
					Id = 1,
					Title = "Первое событие",
					Description = "Очень классное событие",
					StartAt = DateTime.Now,
					EndAt = DateTime.Now.AddHours(2)
				}
			];

		private readonly ILogger<EventController> _logger;
		public EventService(ILogger<EventController> logger) 
		{ 
			_logger = logger;
		}

		public List<Event> GetAll()
		{
			return Events;
		}

		public Event? GetById(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogError(message);
				throw new Exception(message);
			}
			return eventCustom;
		}

		public void Create(Event eventCustom)
		{
			if (Events.Any(e => e.Id == eventCustom.Id))
			{
				var message = $"Cобытие с {eventCustom.Id} уже существует в списке событий!";
				_logger.LogInformation(message);
				throw new Exception(message);
			}
			Events.Add(eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно добавлено в список событий");		
		}

		public void Update(int id, Event eventCustom)
		{
			var index = Events.FindIndex(e => e.Id == id);
			if (index == -1 )
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogInformation(message);
				throw new Exception(message);
			}
			
			Events[index] = eventCustom;
		}

		public void Delete(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				var message = $"Невозможно удалить событие с {id}, т.к его не существует!";
				_logger.LogError(message);
				throw new Exception(message);
			}
			Events.Remove(eventCustom);
			_logger.LogInformation($"Cобытие с {id} успешно удалено");
		}
	}
}
