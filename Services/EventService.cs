using WebApiTamakulov.Controllers;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
	public class EventService: IEventService
	{
		private static List<Event> Events { get; set; } = [];

		private readonly ILogger<EventController> _logger;
		public EventService(ILogger<EventController> logger) 
		{ 
			_logger = logger;
		}
	}
}
