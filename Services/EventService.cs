using System.ComponentModel.DataAnnotations;
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
		private readonly ILogger<EventService> _logger;
		private readonly IEventRepository _eventRepository;
		public EventService(ILogger<EventService> logger, IEventRepository eventRepository)
		{
			_logger = logger;
			_eventRepository = eventRepository;
		}

		public PaginatedResult GetAll(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
		{	
			var filteredEvent = _eventRepository.GetEvents().AsEnumerable();

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

		public Event? GetById(Guid id)
		{
			var eventCustom = _eventRepository.GetEventById(id);
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
			if (eventCustom.TotalSeats <= 0)
			{
				throw new ValidationException($"TotalSeats должно быть больше нуля. Указано значение: {eventCustom.TotalSeats}");
			}

			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

			var events = _eventRepository.GetEvents();
			if (events.Any(e => e.Id == eventCustom.Id))
			{
				var message = $"Cобытие с {eventCustom.Id} уже существует в списке событий!";
				_logger.LogInformation(message);
				return false;
			}
			_eventRepository.AddEvent(eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно добавлено в список событий");
			return true;
		}

		public bool Update(Guid id, Event eventCustom)
		{
			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

			var evenst = _eventRepository.GetEvents();
			var index = evenst.FindIndex(e => e.Id == id);
			if (index == -1)
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogInformation(message);
				return false;
			}

			_eventRepository.UpdateEventByIndex(index, eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно обновлено");
			return true;
		}

		/// <summary>
		/// Удаляет событие по Id.
		/// </summary>
		/// <param name="id">Id удаляемого события.</param>
		/// <returns>True - если удаление прошло успешно.</returns>
		public bool Delete(Guid id)
		{
			var eventCustom = _eventRepository.GetEventById(id);
			if (eventCustom == null)
			{
				var message = $"Невозможно удалить событие с {id}, т.к его не существует!";
				_logger.LogError(message);
				return false;
			}
			_eventRepository.DeleteEventById(eventCustom.Id);
			_logger.LogInformation($"Cобытие с {id} успешно удалено");
			return true;
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

		/// <summary>
		/// Попытка резервирования места на событие.
		/// </summary>
		/// <param name="count">Количество для резервации.</param>
		/// <returns>Возвращает false, если свободных мест недостаточно.</returns>
		public bool TryReserveSeats(Guid id, int count = 1)
		{
			var eventCustom = _eventRepository.GetEventById(id);
			if (eventCustom == null)
			{
				_logger.LogError($"Событие {id} не сущетвует!");
				return false;
			}
			if (eventCustom.AvailableSeats >= count)
			{
				eventCustom.AvailableSeats -= count;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Отмена резервирования места на событие.
		/// </summary>
		/// <param name="count">Количество мест для отмены.</param>
		/// <returns></returns>
		public bool ReleaseSeats(Guid id, int count = 1)
		{
			var eventCustom = _eventRepository.GetEventById(id);
			if (eventCustom == null)
			{
				_logger.LogError($"Событие {id} не сущетвует!");
				return false;
			}
			if (eventCustom.AvailableSeats + count <= eventCustom.TotalSeats )
			{
				eventCustom.AvailableSeats += count;
				return true;
			}
			return false;
		}
	}
	#pragma warning restore CS1591
}
