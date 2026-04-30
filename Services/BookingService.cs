using OpenQA.Selenium;
using System.Net.NetworkInformation;
using WebApiTamakulov.ExceptionExtension;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Services
{

#pragma warning disable CS1591

	public class BookingService : IBookingService
	{
		private readonly ILogger<BookingService> _logger;
		private readonly IEventService _eventService;
		private readonly IBookingRepository _bookingRepository;

		public BookingService(ILogger<BookingService> logger, IEventService eventService, IBookingRepository bookingRepository)
		{
			_logger = logger;
			_eventService = eventService;
			_bookingRepository = bookingRepository;
		}

		public async Task<Booking> CreateBookingAsync(Guid eventId)
		{
			var resultReserve = false;
			resultReserve = _eventService.TryReserveSeats(eventId);

			if (!resultReserve)
				throw new NoAvailableSeatsException();

			var newBooking = _bookingRepository.AddBooking(eventId);

			var message = $"Бронирования для события с eventId = {eventId} созданно!";
			_logger.LogInformation(message);

			return newBooking;
		}

		public async Task<Booking> GetBookingByIdAsync(Guid bookingId)
		{
			var booking = _bookingRepository.GetBookingById(bookingId);
			if (booking == null)
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return default!;
			}
			if (_eventService.GetById(booking.EventId) == null)
			{
				_logger.LogInformation($"Событие c EventId {booking.EventId} не существует! Бронирование {booking.Id} будет удалено");
				_bookingRepository.DeleteBookingById(booking.Id);
				return default!;
			}
			_logger.LogInformation($"Найдено бронирование с id = {bookingId}");
			return booking;
		}

		public List<Booking> GetAllPendingStatusBookingAsync()
		{
			return _bookingRepository.GetBookingsByStatus(Enums.BookingStatus.Pending);
		}

		public void ConfirmBookingAsync(Guid id)
		{
			_bookingRepository.UpdateBooking(id, Enums.BookingStatus.Confirmed);
		}

		public void RejectedBookingAsync(Guid bookingId, Guid? eventId = default)
		{
			_bookingRepository.UpdateBooking(bookingId, Enums.BookingStatus.Rejected);
			if (eventId != null)
				_eventService.ReleaseSeats(eventId.Value);
		}
	}

#pragma warning restore CS1591
}
