using WebApiTamakulov.Enums;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{
#pragma warning disable CS1591

	public class BookingRepository : IBookingRepository
	{
		private static List<Booking> Bookings { get; set; } = [];

		public Booking AddBooking(Guid eventId)
		{
			var booking = new Booking(eventId);
			Bookings.Add(booking);
			return booking;
		}

		public void DeleteBookingById(Guid bookingId)
		{
			var booking = Bookings.FirstOrDefault(b => b.Id == bookingId);
			if (booking != null)
				Bookings.Remove(booking);
		}

		public Booking? GetBookingById(Guid bookingId)
		{
			return Bookings.FirstOrDefault(b => b.Id == bookingId);
		}

		public List<Booking> GetBookingsByStatus(BookingStatus status)
		{
			return Bookings.Where(b => b.Status == status).ToList();
		}

		public void UpdateBooking(Guid id, BookingStatus status)
		{
			var booking = Bookings.FirstOrDefault(b => b.Id == id);
			if (booking != null)
			{
				booking.Status = status;
				booking.ProcessedAt = DateTime.Now;
			}
		}

		public void Reset()
		{
			Bookings.Clear();
		}
	}

#pragma warning restore CS1591
}
