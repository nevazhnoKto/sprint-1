using Microsoft.EntityFrameworkCore;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
#pragma warning disable CS1591
	public class EventRepository : IEventRepository
	{
		private readonly AppDbContext _context;
		/*private static List<Event> Events { get; set; } =
			[
				new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10)
			];*/

		public EventRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task AddEvent(Event newEvent)
		{
			_context.Events.Add(newEvent);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteEventById(Guid id)
		{
			var findEvent = _context.Events.FirstOrDefault(e => e.Id == id);
			if (findEvent != null)
			{
				_context.Events.Remove(findEvent);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<Event?> GetEventById(Guid id)
		{
			return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
		}

		public async Task<List<Event>> GetEvents()
		{
			return await _context.Events.ToListAsync();
		}


		public async Task<List<Event>> GetEventsFiltered(string? title, DateTime? from, DateTime? to)
		{
			IQueryable<Event> query = _context.Events.AsQueryable();  // Начинаем с IQueryable

			if (!string.IsNullOrEmpty(title))
			{
				query = query.Where(e => e.Title != null &&
										e.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
			}

			if (from.HasValue)
			{
				query = query.Where(e => e.StartAt >= from.Value);
			}

			if (to.HasValue)
			{
				query = query.Where(e => e.EndAt <= to.Value);
			}

			// Выполняем запрос
			var result = await query.ToListAsync();

			return result;
		}

		public async Task<bool> UpdateEventByIndex(Guid index, Event eventCustom)
		{
			var existingEvent = _context.Events.FirstOrDefault(e => e.Id == index);
			
			if (existingEvent != null)
			{
				_context.Events.Update(eventCustom);

				// Обновляем только нужные поля
				existingEvent.Title = eventCustom.Title;
				existingEvent.Description = eventCustom.Description;
				existingEvent.StartAt = eventCustom.StartAt;
				existingEvent.EndAt = eventCustom.EndAt;
				existingEvent.TotalSeats = eventCustom.TotalSeats;
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}

		/*public void Reset()
		{
			Events =
			[
				new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10)
			];
		}*/
	}
#pragma warning restore CS1591
}
