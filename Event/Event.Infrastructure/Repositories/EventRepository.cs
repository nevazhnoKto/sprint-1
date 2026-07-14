
using Event.Application.Interfaces;
using Event.Domain.Models;
using Event.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Event.Infrastructure.Repositories
{
#pragma warning disable CS1591
	public class EventRepository : IEventRepository
	{
		private readonly AppDbContext _context;

		public EventRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task AddEvent(EventModel newEvent)
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

		public async Task<EventModel?> GetEventById(Guid id)
		{
			return await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
		}
		public async Task UpdateAsync(EventModel eventCustom)
		{
			_context.Events.Update(eventCustom);
			await _context.SaveChangesAsync();
		}

		public async Task<List<EventModel>> GetEvents()
		{
			return await _context.Events.ToListAsync();
		}


		public async Task<List<EventModel>> GetEventsFiltered(string? title, DateTime? from, DateTime? to)
		{
			IQueryable<EventModel> query = _context.Events.AsQueryable();  // Начинаем с IQueryable

			if (!string.IsNullOrEmpty(title))
			{
				query = query.Where(e => e.Title != null &&
										 e.Title.ToLower().Contains(title.ToLower()));
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

		public async Task<bool> UpdateEventByIndex(Guid index, EventModel eventCustom)
		{
			var existingEvent = _context.Events.FirstOrDefault(e => e.Id == index);
			
			if (existingEvent != null)
			{
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

		public async  Task<List<EventModel>> GetTop10EventsAsync()
		{
			return await _context.Events.OrderByDescending(e => (double)(e.TotalSeats - e.AvailableSeats) / e.TotalSeats).Take(10).ToListAsync();
		}
	}
#pragma warning restore CS1591
}
