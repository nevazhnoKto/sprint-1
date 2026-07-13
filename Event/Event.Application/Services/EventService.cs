using Event.Application.Interfaces;
using Event.Application.Models;
using Event.Domain.ExceptionExtension;
using Event.Domain.Models;
using MapsterMapper;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Event.Application.Services
{
#pragma warning disable CS1591

	/// <summary>
	/// Сервис обработки событий.
	/// </summary>
	public class EventService : IEventService
	{
		private readonly ILogger<EventService> _logger;
		private readonly IEventRepository _eventRepository;
		private readonly IMapper _mapper;
		private readonly IRedisService _redis;

		public EventService(ILogger<EventService> logger, IEventRepository eventRepository, IMapper mapper, IRedisService redis)
		{
			_logger = logger;
			_eventRepository = eventRepository;
			_mapper = mapper;
			_redis = redis;
		}

		public async Task<PaginatedResult> GetAll(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10)
		{
			var filteredEvent = await _eventRepository.GetEventsFiltered(title, from, to);

			var paginatedFilteredEvent = GetPage(filteredEvent, page, pageSize).ToList();
			var filteredEventCount = filteredEvent.Count;

			_logger.LogInformation($"Получение всех отфильтрованных событий, количество = {filteredEventCount}");

			var paginatedResult = new PaginatedResult()
			{
				TotalCount = filteredEventCount,
				Items = _mapper.Map<List<EventDto>>(paginatedFilteredEvent),
				CurrentPage = page,
				CountCurrentPage = paginatedFilteredEvent.Count
			};

			return paginatedResult;
		}

		private IEnumerable<EventModel> GetPage(IEnumerable<EventModel> events, int page, int pageSize)
		{
			return events.OrderByDescending(e => e.StartAt)
						 .Skip((page - 1) * pageSize)
						 .Take(pageSize);
		}

		public async Task<EventDto?> GetById(Guid id)
		{
			// Получить из кэша.
			var eventCustom = await _redis.GetCacheForIdAsync(id);
			if(eventCustom == null)
			{
				eventCustom = await _eventRepository.GetEventById(id);
				if (eventCustom == null)
				{
					var message = $"Cобытия с {id} не существует!";
					_logger.LogInformation(message);
					return default;
				}

				// Прогреть кэш.
				await _redis.SetCacheAsync(id, eventCustom);
			}

			_logger.LogInformation($"Найдено событие с id = {id}");
			return _mapper.Map<EventDto>(eventCustom);
		}

		public async Task<bool> Create(CreateEventRequestDto eventCustomDto)
		{
			var eventCustom = _mapper.Map<EventModel>(eventCustomDto);
			if (eventCustom.TotalSeats <= 0)
			{
				throw new ValidationException($"TotalSeats должно быть больше нуля. Указано значение: {eventCustom.TotalSeats}");
			}

			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

			var events = await _eventRepository.GetEvents();
			if (events.Any(e => e.Id == eventCustom.Id))
			{
				var message = $"Cобытие с {eventCustom.Id} уже существует в списке событий!";
				_logger.LogInformation(message);
				return false;
			}
			await _eventRepository.AddEvent(eventCustom);
			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно добавлено в список событий");

			// Прогреть кэш.
			await _redis.SetCacheAsync(eventCustom.Id, eventCustom);
			return true;
		}

		public async Task<bool> Update(Guid id, UpdateEventRequestDto updateCustomDto)
		{
			var eventCustom = _mapper.Map<EventModel>(updateCustomDto);
			if (!ValidateDate(eventCustom.StartAt, eventCustom.EndAt))
			{
				return false;
			}

			var result = await _eventRepository.UpdateEventByIndex(id, eventCustom);

			if (!result)
			{
				var message = $"Cобытия с {id} не существует!";
				_logger.LogInformation(message);
				return false;
			}

			_logger.LogInformation($"Cобытие с id = {eventCustom.Id} успешно обновлено");

			// Прогреть кэш.
			await _redis.SetCacheAsync(eventCustom.Id, eventCustom);
			return true;
		}

		/// <summary>
		/// Удаляет событие по Id.
		/// </summary>
		/// <param name="id">Id удаляемого события.</param>
		/// <returns>True - если удаление прошло успешно.</returns>
		public async Task<bool> Delete(Guid id)
		{
			// Получить из кэша.
			var eventCustom = await _redis.GetCacheForIdAsync(id);
			if (eventCustom == null)
			{
				eventCustom = await _eventRepository.GetEventById(id);
				if (eventCustom == null)
				{
					var message = $"Невозможно удалить событие с {id}, т.к его не существует!";
					_logger.LogError(message);
					return false;
				}
			}

			await _eventRepository.DeleteEventById(eventCustom.Id);
			_logger.LogInformation($"Cобытие с {id} успешно удалено");

			// Удалить из кэша.
			await _redis.DeleteCacheAsync(id);
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

		public async Task<bool> TryReserveSeats(Guid id, int count = 1)
		{
			var eventCustom = await _redis.GetCacheForIdAsync(id);
			if (eventCustom == null)
			{
				eventCustom = await _eventRepository.GetEventById(id);
				if (eventCustom == null)
				{
					throw new EventDoesNotExist($"События {id} не существует!");
				}
			}

			var result = await eventCustom.TryReserveSeats(count);
			if (result)
			{
				await _eventRepository.UpdateAsync(eventCustom);
				// Прогреть кэш.
				await _redis.SetCacheAsync(id, eventCustom);
			}

			return result;
		}

		public async Task<bool> ReleaseSeats(Guid id, int count = 1)
		{
			var eventCustom = await _redis.GetCacheForIdAsync(id);
			if (eventCustom == null)
			{
				eventCustom = await _eventRepository.GetEventById(id);
				if (eventCustom == null)
				{
					throw new EventDoesNotExist($"События {id} не существует!");
				}
			}

			var result = await eventCustom.ReleaseSeats(count);
			if (result)
			{
				await _eventRepository.UpdateAsync(eventCustom);
				// Прогреть кэш.
				await _redis.SetCacheAsync(id, eventCustom);
			}

			return result;
		}
	}
#pragma warning restore CS1591
}
