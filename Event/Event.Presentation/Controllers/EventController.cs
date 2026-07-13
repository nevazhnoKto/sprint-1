using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Event.Presentation.Models;
using System.Net;
using Event.Application.Interfaces;
using Event.Application.Models;

namespace Event.Presentation.Controllers
{
	/// <summary>
	/// Api контроллер для работы с Событиями.
	/// </summary>
	[ApiController]
	[Route("events")]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;

		/// <summary>
		/// Api контроллер для работы с Событиями.
		/// </summary>
		/// <param name="eventService">Сервис для работы с Событиями.</param>
		/// <param name="mapper">Маппер.</param>
		public EventController(IEventService eventService, IMapper mapper)
		{
			_eventService = eventService;
		}

		/// <summary>
		/// Метод возвращает все существующие Event.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PaginatedResultDto), StatusCodes.Status200OK)]
		[Produces("application/json")]
		public async Task<IActionResult> GetAllEvents(GetEventsRequestDto eventsRequest)
		{
			var events = await _eventService.GetAll(eventsRequest.Title, eventsRequest.From, eventsRequest.To, eventsRequest.Page, eventsRequest.PageSize);

			var eventsDto = new PaginatedResultDto()
			{
				TotalCount = events.TotalCount,
				CurrentPage = events.CurrentPage,
				CountCurrentPage = events.CountCurrentPage,
				Items = events.Items
			};

			return Ok(new ApiResult<PaginatedResultDto>()
			{
				Success = true,
				Data = eventsDto,
				StatusCode = HttpStatusCode.OK,
				Message = "Вернул все Events с заданными фильтрамиы."
			});
		}

		/// <summary>
		/// Возвращает 10 событий с наибольшим процентом проданных мест.
		/// </summary>
		/// <returns>Список событий.</returns>
		[HttpGet]
		[ProducesResponseType(typeof(List<EventDto>), StatusCodes.Status200OK)]
		[Produces("application/json")]
		public async Task<IActionResult> GetTop10Events()
		{
			var listEvents = await _eventService.GetTop10Events();

			return Ok(new ApiResult<List<EventDto>>()
			{
				Success = true,
				Data = listEvents!,
				StatusCode = HttpStatusCode.OK,
				Message = listEvents.Any() ? "Вернул 10 событий с наибольшим процентом проданных мест." : "Список событий пуст!"
			});
		}

		/// <summary>
		/// Метод возвращает Event по запрашиваемому Id.
		/// </summary>
		/// <param name="id">Запрашиваемый Id события.</param>
		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(ApiResult<EventDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> GetByIdEvent(Guid id)
		{

			var eventById = await _eventService.GetById(id);

			if (eventById != null)
			{
				return Ok(new ApiResult<EventDto>()
				{
					Success = true,
					Data = eventById,
					StatusCode = HttpStatusCode.OK,
					Message = $"Вернул Event по id = {id}."
				});
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event по id = {id} не существует."
			});
		}

		/// <summary>
		/// Метод создает новый Event.
		/// </summary>
		/// <param name="newEventDto">Данные нового Event.</param>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(typeof(ApiResult<CreateEventRequestDto>), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequestDto newEventDto)
		{
			if (await _eventService.Create(newEventDto))
			{
				var response = new ApiResult<CreateEventRequestDto>
				{
					Success = true,
					Data = newEventDto,
					StatusCode = HttpStatusCode.Created,
					Message = $"Создался Event по id = {newEventDto.Id}."
				};
				return CreatedAtAction(nameof(GetByIdEvent), new { id = newEventDto.Id }, response);
			}

			return BadRequest(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.BadRequest,
				Message = $"Event по id = {newEventDto.Id} уже существует."
			});
		}

		/// <summary>
		/// Метод обновляет существующий Event по переданному Id.
		/// </summary>
		/// <param name="id">Id события для обновления.</param>
		/// <param name="updateEventDto">Event для обновления.</param>
		[HttpPut("{id:Guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventRequestDto updateEventDto)
		{

			if (await _eventService.Update(id, updateEventDto))
			{
				return NoContent();
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event с id = {id} не найден."
			});
		}

		/// <summary>
		/// Метод удаляет существующий Event по переданному Id.
		/// </summary>
		/// <param name="id">Id события для удаления.</param>
		[HttpDelete("{id:Guid}")]
		[Authorize(Roles = "Admin")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> DeleteEvent(Guid id)
		{

			if (await _eventService.Delete(id))
			{
				return NoContent();
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event с id = {id} не найден."
			});
		}
	}
}
