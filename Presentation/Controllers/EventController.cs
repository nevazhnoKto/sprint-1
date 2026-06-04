using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Application.Interfaces;
using Application.Models;
using Presentation.Models;

namespace Presentation.Controllers
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
					Message = $"Создался Event по id = {newEventDto.Id}"
				};
				return CreatedAtAction(nameof(GetByIdEvent), new { id = newEventDto.Id }, response);
			}

			return BadRequest(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.BadRequest,
				Message = $"Event по id = {newEventDto.Id} уже существует"
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
				Message = $"Event с id = {id} не найден"
			});
		}
	}
}
