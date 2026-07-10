using Microsoft.EntityFrameworkCore;
using Booking.Infrastructure.DataAccess;
using Booking.Domain.Enums;
using Booking.Application.Interfaces;
using Booking.Domain.Models;

namespace Booking.Infrastructure.Repositories
{
#pragma warning disable CS1591

	public class BookingRepository : IBookingRepository
	{
		private readonly AppDbContext _context;
		public BookingRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task<BookingModel> AddBooking(Guid eventId, Guid userId)
		{
			var booking = new BookingModel(eventId, userId);
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

		public async Task<BookingModel?> GetBookingById(Guid bookingId)
		{
			return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
		}
		public async Task<BookingModel?> GetBookingById(Guid bookingId, Guid userId)
		{
			return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId);
		}

		public async Task<List<BookingModel>> GetBookingsByStatus(BookingStatus status)
		{
			return await _context.Bookings.Where(b => b.Status == status).ToListAsync();
		}

		public async Task<int> GetCountBookingByUserId(Guid userId)
		{
			return await _context.Bookings.AsNoTracking().CountAsync(b => b.UserId == userId && b.Status != BookingStatus.Cancelled);
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
