using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class EventController : ControllerBase
	{
		private readonly IEventService _eventService;
		private readonly IMapper _mapper;
		public EventController(IEventService eventService, IMapper mapper)
		{
			_eventService = eventService;
			_mapper = mapper;
		}

		/// <summary>
		/// Метод возвращает все существующие Event.
		/// </summary>			
		[HttpGet("events")]
		[ProducesResponseType(typeof(ApiBaseResult), StatusCodes.Status200OK)]		
		public ApiResult<List<EventDto>> GetAllEvents()
		{
			var result = _eventService.GetAll().Select(e => _mapper.Map<EventDto>(e)).ToList();			
			return new ApiResult<List<EventDto>>()
			{
				Success = true,
				Data = result,
				StatusCode = System.Net.HttpStatusCode.OK,
				Message = "Вернул все Events"
			};
			
		}

		/// <summary>
		/// Метод возвращает Event по запрашиваемому Id.
		/// </summary>	
		[HttpGet("events/{id}")]
		[ProducesResponseType(typeof(ApiBaseResult), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
		public ApiBaseResult GetByIdEvent(int id)
		{
			try
			{
				var eventById = _eventService.GetById(id);
				
				return new ApiResult<EventDto>()
				{
					Success = true,
					Data = _mapper.Map<EventDto>(eventById),
					StatusCode = System.Net.HttpStatusCode.OK,
					Message = $"Вернул Event по id = {id}"
				};
			}
			catch (Exception ex)
			{
				return new ApiResult()
				{
					Success = false,					
					StatusCode = System.Net.HttpStatusCode.NotFound,
					Message = $"{ex.Message}"
				};
			}
			
		}

		/// <summary>
		/// Метод создает новый Event.
		/// </summary>	
		[HttpPost("events")]
		[ProducesResponseType(typeof(ApiBaseResult), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
		public ApiBaseResult CreateEvent([FromBody] EventDto newEvent)
		{
			try
			{
				_eventService.Create(_mapper.Map<Event>(newEvent));
				return new ApiResult<CreatedAtActionResult>()
				{
					Success = true,
					Data = CreatedAtAction( actionName: nameof(CreateEvent),
											routeValues: new { id = newEvent.Id },
											value: newEvent),
					StatusCode = System.Net.HttpStatusCode.Created,
					Message = $"Создался Event по id = {newEvent.Id}"
				};
			}
			catch(Exception ex)
			{
				return new ApiResult()
				{
					Success = false,
					StatusCode = System.Net.HttpStatusCode.BadRequest,
					Message = $"{ex.Message}"
				};
			}
		}

		/// <summary>
		/// Метод обновляет существующий Event по переданному Id. 
		/// </summary>	
		[HttpPut("events/{id}")]
		[ProducesResponseType(typeof(ApiBaseResult), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
		public ApiBaseResult UpdateEvent(int id, [FromBody] EventDto updateEvent)
		{
			try
			{
				_eventService.Update(id, _mapper.Map<Event>(updateEvent));

				return new ApiResult()
				{
					Success = true,					
					StatusCode = System.Net.HttpStatusCode.OK,
					Message = $"Обновил Event по id = {id}"
				};
			}
			catch (Exception ex)
			{
				return new ApiResult()
				{
					Success = false,
					StatusCode = System.Net.HttpStatusCode.NotFound,
					Message = $"{ex.Message}"
				};
			}
		}

		/// <summary>
		/// Метод удаляет существующий Event по переданному Id. 
		/// </summary>	
		[HttpDelete("events/{id}")]
		[ProducesResponseType(typeof(ApiBaseResult), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
		public ApiBaseResult DeleteEvent(int id)
		{
			try
			{
				_eventService.Delete(id);

				return new ApiResult()
				{
					Success = true,
					StatusCode = System.Net.HttpStatusCode.OK,
					Message = $"Удалил Event по id = {id}"
				};
			}
			catch (Exception ex)
			{
				return new ApiResult()
				{
					Success = false,
					StatusCode = System.Net.HttpStatusCode.NotFound,
					Message = $"{ex.Message}"
				};
			}
		}
	}
}
