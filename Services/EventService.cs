using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Linq.Expressions;
using WebApiTamakulov.Controllers;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
	public class EventService: IEventService
	{
		private static List<Event> Events { get; set; } = [];

		private readonly ILogger<EventController> _logger;
		public EventService(ILogger<EventController> logger) 
		{ 
			_logger = logger;
		}

		public IEnumerable<Event> GetAll()
		{
			return Events;
		}

		public Event? GetById(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				_logger.LogError($"Cобытия с {id} не существует!");
				return default;
			}
			return eventCustom;
		}

		public bool Create(Event eventCustom)
		{
			if (Events.Any(e => e.Id == eventCustom.Id))
			{
				_logger.LogInformation($"Cобытие с {eventCustom.Id} уже существует в списке событий!");
				return default;
			}
			Events.Add(eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно добавлено в список событий");
			return true;
		}

		public bool Update(int id, Event eventCustom)
		{
			if (!Events.Any(e => e.Id == eventCustom.Id))
			{
				_logger.LogInformation($"Cобытия с {id} не существует!");
				return default;
			}
			Events[id] = eventCustom;
			return true;
		}

		public bool Delete(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				_logger.LogError($"Невозможно удалить событие с {id}, т.к его не существует!");
				return default;
			}
			Events.Remove(eventCustom);
			_logger.LogInformation($"Cобытие с {id} успешно удалено");
			return true;
			
		}
	}
}
