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
		public EventController(IEventService eventService)
		{
			_eventService = eventService;
		}

		[HttpGet("events")]
		public IEnumerable<Event> GetAllEvents()
		{
			
		}

		[HttpGet("events/{id}")]
		public Event GetByIdEvent(int id)
		{

		}

		[HttpPost("events")]
		public ActionResult CreateEvent([FromBody] Event newEvent)
		{
			return Ok();
		}

		[HttpPut("events/{id}")]
		public ActionResult UpdateEvent([FromBody] int id, Event updateEvent)
		{
			return Ok();
		}

		[HttpDelete("events/{id}")]
		public ActionResult DeleteEvent(int id)
		{
			return Ok();
		}
	}
}
