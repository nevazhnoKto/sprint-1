using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Controllers
{
	/// <summary>
	/// Api контроллер для работы с Событиями.
	/// </summary>
	[ApiController]
	[Route("events")]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;
		private readonly IMapper _mapper;
		/// <summary>
		/// Api контроллер для работы с Событиями.
		/// </summary>
		/// <param name="eventService">Сервис для работы с Событиями.</param>
		/// <param name="mapper">Маппер.</param>
		public EventController(IEventService eventService, IMapper mapper)
		{
			_eventService = eventService;
			_mapper = mapper;
		}

		/// <summary>
		/// Метод возвращает все существующие Event.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PaginatedResultDto), StatusCodes.Status200OK)]
		[Produces("application/json")]
		public IActionResult GetAllEvents(GetEventsRequestDto eventsRequest)
		{
			var events = _eventService.GetAll(eventsRequest.Title, eventsRequest.From, eventsRequest.To, eventsRequest.Page, eventsRequest.PageSize);

			var eventsDto = new PaginatedResultDto()
			{
				TotalCount = events.TotalCount,
				CurrentPage = events.CurrentPage,
				CountCurrentPage = events.CountCurrentPage,
				Items = _mapper.Map<List<EventResponseDto>>(events.Items)
			};

			return Ok(new ApiResult<PaginatedResultDto>()
			{
				Success = true,
				Data = eventsDto,
				StatusCode = HttpStatusCode.OK,
				Message = "Вернул все Events с заданными фильтрамиы"
			});
		}

		/// <summary>
		/// Метод возвращает Event по запрашиваемому Id.
		/// </summary>
		/// <param name="id">Запрашиваемый Id события.</param>
		[HttpGet("{id:Guid}")]
		[ProducesResponseType(typeof(ApiResult<EventResponseDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public IActionResult GetByIdEvent(Guid id)
		{

			var eventById = _eventService.GetById(id);

			if (eventById != null)
			{
				return Ok(new ApiResult<EventResponseDto>()
				{
					Success = true,
					Data = _mapper.Map<EventResponseDto>(eventById),
					StatusCode = HttpStatusCode.OK,
					Message = $"Вернул Event по id = {id}"
				});
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event по id = {id} не существует"
			});
		}

		/// <summary>
		/// Метод создает новый Event.
		/// </summary>
		/// <param name="newEventDto">Данные нового Event.</param>
		[HttpPost]
		[ProducesResponseType(typeof(ApiResult<EventResponseDto>), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public IActionResult CreateEvent([FromBody] CreateEventRequestDto newEventDto)
		{
			var newEvent = _mapper.Map<Event>(newEventDto);
			if (_eventService.Create(newEvent))
			{
				var response = new ApiResult<EventResponseDto>
				{
					Success = true,
					Data = _mapper.Map<EventResponseDto>(newEvent),
					StatusCode = HttpStatusCode.Created,
					Message = $"Создался Event по id = {newEventDto.Id}"
				};
				return CreatedAtAction(nameof(GetByIdEvent), new { id = newEvent.Id }, response);
			}

			return BadRequest(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.BadRequest,
				Message = $"Event по id = {newEvent.Id} уже существует"
			});
		}

		/// <summary>
		/// Метод обновляет существующий Event по переданному Id.
		/// </summary>
		/// <param name="id">Id события для обновления.</param>
		/// <param name="updateEventDto">Event для обновления.</param>
		[HttpPut("{id:Guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public IActionResult UpdateEvent(Guid id, [FromBody] UpdateEventRequestDto updateEventDto)
		{

			if (_eventService.Update(id, _mapper.Map<Event>(updateEventDto)))
			{
				return NoContent();
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event с id = {id} не найден"
			});
		}

		/// <summary>
		/// Метод удаляет существующий Event по переданному Id.
		/// </summary>
		/// <param name="id">Id события для удаления.</param>
		[HttpDelete("{id:Guid}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public IActionResult DeleteEvent(Guid id)
		{

			if (_eventService.Delete(id))
			{
				return NoContent();
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event с id = {id} не найден"
			});
		}
	}
}
