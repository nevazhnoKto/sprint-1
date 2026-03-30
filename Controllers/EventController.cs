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
		private readonly IBookingService _bookingService;
		private readonly IMapper _mapper;
		/// <summary>
		/// Api контроллер для работы с Событиями.
		/// </summary>
		/// <param name="eventService">Сервис для работы с Событиями.</param>
		/// <param name="mapper">Маппер.</param>
		/// <param name="bookingService">Сервис для работы с бронированием.</param>
		public EventController(IEventService eventService, IMapper mapper, IBookingService bookingService)
		{
			_eventService = eventService;
			_mapper = mapper;
			_bookingService = bookingService;
		}

		/// <summary>
		/// Метод возвращает все существующие Event.
		/// </summary>
		[HttpGet]
		[ProducesResponseType(typeof(PaginatedResult), StatusCodes.Status200OK)]
		[Produces("application/json")]
		public IActionResult GetAllEvents(GetEventsRequest eventsRequest)
		{
			var events = _eventService.GetAll(eventsRequest.Title, eventsRequest.From, eventsRequest.To, eventsRequest.Page, eventsRequest.PageSize);

			return Ok(new ApiResult<PaginatedResult>()
			{
				Success = true,
				Data = events,
				StatusCode = HttpStatusCode.OK,
				Message = "Вернул все Events с заданными фильтрамиы"
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
		public IActionResult GetByIdEvent(Guid id)
		{

			var eventById = _eventService.GetById(id);

			if (eventById != null)
			{
				return Ok(new ApiResult<EventDto>()
				{
					Success = true,
					Data = _mapper.Map<EventDto>(eventById),
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
		[ProducesResponseType(typeof(ApiResult<EventDto>), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[Produces("application/json")]
		public IActionResult CreateEvent([FromBody] EventDto newEventDto)
		{
			var newEvent = _mapper.Map<Event>(newEventDto);
			if (_eventService.Create(newEvent))
			{
				var response = new ApiResult<EventDto>
				{
					Success = true,
					Data = newEventDto,
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
		public IActionResult UpdateEvent(Guid id, [FromBody] EventDto updateEventDto)
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

		/// <summary>
		/// Метод создает новое бронирования для конкретного EventId.
		/// </summary>
		/// <param name="id">Id события для бронирования.</param>
		[HttpPost("{id:Guid}/book")]
		[ProducesResponseType(typeof(ApiResult<Booking>), StatusCodes.Status202Accepted)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> CreateBooking(Guid id)
		{
			var booking = await _bookingService.CreateBookingAsync(id);
			if (booking != null)
			{
				var response = new ApiResult<Booking>
				{
					Success = true,
					Data = booking,
					StatusCode = HttpStatusCode.Accepted,
					Message = $"Создалось бронирование для EventId = {id}. BookingId = {booking.Id}"
				};
				return AcceptedAtAction(nameof(GetByIdBooking), new { id = booking.Id }, response);
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event по id = {id} не существует, невозможно создать событие!"
			});
		}

		/// <summary>
		/// Метод возвращает бронирование по запрашиваемому Id.
		/// </summary>
		/// <param name="id">Запрашиваемый Id бронирования.</param>
		[HttpGet("/bookings/{id:Guid}")]
		[ProducesResponseType(typeof(ApiResult<string>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> GetByIdBooking(Guid id)
		{

			var bookingById = await _bookingService.GetBookingByIdAsync(id);

			if (bookingById != null)
			{
				return Ok(new ApiResult<string>()
				{
					Success = true,
					Data = bookingById.Status.ToString(),
					StatusCode = HttpStatusCode.OK,
					Message = $"Вернул бронирование по id = {id}"
				});
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Бронирование по id = {id} не существует"
			});
		}

	}
}
