using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{

#pragma warning disable CS1591

	public class BookingService : IBookingService
	{
		private static List<Booking> Bookings { get; set; } = [];

		private readonly ILogger<BookingService> _logger;

		public BookingService(ILogger<BookingService> logger)
		{
			_logger = logger;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId)
		{
			var newBooking = new Booking()
			{
				Id = Guid.NewGuid(),
				EventId = eventId,
				Status = Enums.BookingStatus.Pending,
				CreatedAt = DateTime.Now,
				ProcessedAt = null,
			};

			Bookings.Add(newBooking);

			var message = $"Бронирования для события с eventId = {eventId} созданно!";
			_logger.LogInformation(message);

			return newBooking;
		}

		public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
		{
			var eventCustom = Bookings.FirstOrDefault(e => e.Id == bookingId);
			if (eventCustom == null)
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return default!;
			}
			_logger.LogInformation($"Найдено бронирование с id = {bookingId}");
			return eventCustom;
		}

		public async Task<List<Booking>> GetAllPendingStatusBookingAsync()
		{
			return Bookings.Where(b => b.Status == Enums.BookingStatus.Pending).ToList();
		}

		public async Task UpdateStatusBookingAsync(Guid id, Enums.BookingStatus status)
		{
			var booking = Bookings.FirstOrDefault(b => b.Id == id);
			if (booking != null)
			{
				booking.Status = status;
				booking.ProcessedAt = DateTime.Now;
			}
		}
	}

#pragma warning restore CS1591
}
