using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{

#pragma warning disable CS1591

	public class BookingService : IBookingService
	{
		private static List<Booking> Bookings { get; set; } = [];

		private readonly ILogger<BookingService> _logger;
		private readonly IEventService _eventService;

		public BookingService(ILogger<BookingService> logger, IEventService eventService)
		{
			_logger = logger;
			_eventService = eventService;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId)
		{
			if (_eventService.GetById(eventId) == null)
			{
				return default!;
			}
			var newBooking = new Booking(eventId);

			Bookings.Add(newBooking);

			var message = $"Бронирования для события с eventId = {eventId} созданно!";
			_logger.LogInformation(message);

			return newBooking;
		}

		public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
		{
			var booking = Bookings.FirstOrDefault(e => e.Id == bookingId);
			if (booking == null)
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return default!;
			}
			if (_eventService.GetById(booking.EventId) == null)
			{
				_logger.LogInformation($"Событие c EventId {booking.EventId} не существует! Бронирование {booking.Id} будет удалено");
				Bookings.Remove(booking);
				return default!;
			}
			_logger.LogInformation($"Найдено бронирование с id = {bookingId}");
			return booking;
		}

		public List<Booking> GetAllPendingStatusBookingAsync()
		{
			return Bookings.Where(b => b.Status == Enums.BookingStatus.Pending).ToList();
		}

		public void UpdateStatusBookingAsync(Guid id, Enums.BookingStatus status)
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
