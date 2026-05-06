using Microsoft.EntityFrameworkCore;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Enums;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Repositories
{
#pragma warning disable CS1591

	public class BookingRepository : IBookingRepository
	{
		private readonly AppDbContext _context;
		public BookingRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task<Booking> AddBooking(Guid eventId)
		{
			var booking = new Booking(eventId);
			_context.Bookings.Add(booking);
			await _context.SaveChangesAsync();
			return booking;
		}

		public async Task DeleteBookingById(Guid bookingId)
		{
			var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
			if (booking != null)
			{
				_context.Bookings.Remove(booking);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<Booking?> GetBookingById(Guid bookingId)
		{
			return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
		}

		public async Task<List<Booking>> GetBookingsByStatus(BookingStatus status)
		{
			return await _context.Bookings.Where(b => b.Status == status).ToListAsync();
		}

		public async Task UpdateBooking(Guid id, BookingStatus status)
		{
			var booking = _context.Bookings.FirstOrDefault(b => b.Id == id);
			if (booking != null)
			{
				booking.Status = status;
				booking.ProcessedAt = DateTime.UtcNow;
				await _context.SaveChangesAsync();
			}
		}
	}

#pragma warning restore CS1591
}
