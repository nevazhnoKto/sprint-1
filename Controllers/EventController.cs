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

		[HttpGet("events")]
		public IActionResult GetAllEvents()
		{
			return Ok(_eventService.GetAll());
		}

		[HttpGet("events/{id}")]
		public IActionResult GetByIdEvent(int id)
		{
			var result = _eventService.GetById(id);
			return result == null ? NotFound() : Ok(result);
		}

		[HttpPost("events")]
		public IActionResult CreateEvent([FromBody] EventDto newEvent)
		{			
			var result = _eventService.Create(_mapper.Map<Event>(newEvent));
			return result ?
				CreatedAtAction(
					actionName: nameof(CreateEvent),
					routeValues: new { id = newEvent.Id },
					value: newEvent) : 
				BadRequest();
		}

		[HttpPut("events/{id}")]
		public IActionResult UpdateEvent(int id, [FromBody] EventDto updateEvent)
		{
			var result = _eventService.Update(id, _mapper.Map<Event>(updateEvent));
			return result ? Ok(updateEvent) : NotFound();
		}

		[HttpDelete("events/{id}")]
		public IActionResult DeleteEvent(int id)
		{
			var result = _eventService.Delete(id);
			return result ? Ok() : NotFound();
		}
	}
}
