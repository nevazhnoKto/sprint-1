using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
	/// <summary>
	/// Сервис обработки событий.
	/// </summary>
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

		private readonly ILogger<EventService> _logger;
		public EventService(ILogger<EventService> logger)
		{
			_logger = logger;
		}

		public List<Event> GetAll()
		{
			_logger.LogInformation($"Получение всех событий, количество = {Events.Count}");
			return Events;
		}

		public Event? GetById(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogInformation(message);
				return default;
			}
			_logger.LogInformation($"Найдено событие с id = {id}");
			return eventCustom;
		}

		public bool Create(Event eventCustom)
		{
			if (Events.Any(e => e.Id == eventCustom.Id))
			{
				var message = $"Cобытие с {eventCustom.Id} уже существует в списке событий!";
				_logger.LogInformation(message);
				return false;
			}
			Events.Add(eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно добавлено в список событий");
			return true;
		}

		public bool Update(int id, Event eventCustom)
		{
			var index = Events.FindIndex(e => e.Id == id);
			if (index == -1)
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogInformation(message);
				return false;
			}

			Events[index] = eventCustom;
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно обновлено");
			return true;
		}

		/// <summary>
		/// Удаляет событие по Id.
		/// </summary>
		/// <param name="id">Id удаляемого события.</param>
		/// <returns>True - если удаление прошло успешно.</returns>
		public bool Delete(int id)
		{
			var eventCustom = Events.FirstOrDefault(e => e.Id == id);
			if (eventCustom == null)
			{
				var message = $"Невозможно удалить событие с {id}, т.к его не существует!";
				_logger.LogError(message);
				return false;
			}
			Events.Remove(eventCustom);
			_logger.LogInformation($"Cобытие с {id} успешно удалено");
			return true;
		}
	}
}
