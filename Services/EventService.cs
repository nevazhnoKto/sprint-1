using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
	#pragma warning disable CS1591

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

		public PaginatedResult GetAll(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
		{	
			var filteredEvent = Events.AsEnumerable();

			if (!string.IsNullOrEmpty(title))
			{
				filteredEvent = filteredEvent.Where(e =>
					e.Title != null && e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
			}	
				
			if (from.HasValue)
			{
				filteredEvent = filteredEvent.Where(e => e.StartAt >= from);
			}

			if (to.HasValue)
			{
				filteredEvent = filteredEvent.Where(e => e.EndAt <= to);
			}

			var paginatedFilteredEvent = GetPage(filteredEvent, page, pageSize).ToList();
			var filteredEventCount = filteredEvent.Count();

			_logger.LogInformation($"Получение всех отфильтрованных событий, количество = {filteredEventCount}");

			var paginatedResult = new PaginatedResult()
			{
				TotalCount = filteredEventCount,
				Items = paginatedFilteredEvent,
				CurrentPage = page,
				CountCurrentPage = paginatedFilteredEvent.Count
			};

			return paginatedResult;
		}

		private IEnumerable<Event> GetPage(IEnumerable<Event> events, int page, int pageSize)
		{
			return events.OrderByDescending(e => e.StartAt)
						 .Skip((page - 1) * pageSize)
						 .Take(pageSize);
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
			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

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
			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

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

		// Метод для сброса состояния
		public void Reset()
		{
			Events =
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
		}

		private bool ValidateDate(DateTime? start, DateTime? end)
		{
			if (start.HasValue && end.HasValue && start >= end)
			{
				_logger.LogError("Дата начала должна быть раньше даты конца");
				return false;
			}
			return true;
		}
	}
	#pragma warning restore CS1591
}
