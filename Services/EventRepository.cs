using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
#pragma warning disable CS1591
	public class EventRepository : IEventRepository
	{
		private static List<Event> Events { get; set; } =
			[
				new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10)
			];

		public void AddEvent(Event newEvent)
		{
			Events.Add(newEvent);
		}

		public void DeleteEventById(Guid id)
		{
			var findEvent = Events.FirstOrDefault(e => e.Id == id);
			if (findEvent != null)
				Events.Remove(findEvent);
		}

		public Event? GetEventById(Guid id)
		{
			return Events.FirstOrDefault(e => e.Id == id);
		}

		public List<Event> GetEvents()
		{
			return Events;
		}

		public void UpdateEventByIndex(int index, Event eventCustom)
		{
			if (index >= 0 || index < Events.Count)
				Events[index] = eventCustom;
		}

		public void Reset()
		{
			Events =
			[
				new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10)
			];
		}
	}
#pragma warning restore CS1591
}
