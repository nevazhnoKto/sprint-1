using Application.Interfaces;
using Application.Models;
using Domain.Common;
using Domain.Enums;
using Domain.ExceptionExtension;
using Domain.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Services
{

#pragma warning disable CS1591

	public class BookingService : IBookingService
	{
		private readonly ILogger<BookingService> _logger;
		private readonly IEventService _eventService;
		private readonly IBookingRepository _bookingRepository;
		private readonly IMapper _mapping;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BookingService(ILogger<BookingService> logger, IEventService eventService, IBookingRepository bookingRepository, IMapper mapping, IHttpContextAccessor httpContextAccessor)
		{
			_logger = logger;
			_eventService = eventService;
			_bookingRepository = bookingRepository;
			_mapping = mapping;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<BookingDto> CreateBookingAsync(Guid eventId)
		{
			// Проверка на то, что событие уже началось.
			var eventFinded = await _eventService.GetById(eventId);
			if (eventFinded.StartAt < DateTime.UtcNow)
				throw new EventAlreadyPassedException();

			// Получить ИД пользователя.
			var idUser = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

			// Проверка на максимально количества броней для пользователя.
			var countBookingByUser = await _bookingRepository.GetCountBookingByUserId(idUser);
			if (countBookingByUser >= CommonConst.LimitBookingForUser)
				throw new ActiveBookingLimitExceededException();

			var resultReserve = await _eventService.TryReserveSeats(eventId);

			if (!resultReserve)
				throw new NoAvailableSeatsException();

			
			var newBooking = await _bookingRepository.AddBooking(eventId, idUser);

			var message = $"Бронирования для события с eventId = {eventId} созданно!";
			_logger.LogInformation(message);

			return _mapping.Map<BookingDto>(newBooking);
		}

		public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId)
		{
			var booking = await _bookingRepository.GetBookingById(bookingId);
			if (booking == null)
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return default!;
			}
			if (await _eventService.GetById(booking.EventId) == null)
			{
				_logger.LogInformation($"Событие c EventId {booking.EventId} не существует! Бронирование {booking.Id} будет удалено");
				await _bookingRepository.DeleteBookingById(booking.Id);
				return default!;
			}
			_logger.LogInformation($"Найдено бронирование с id = {bookingId}");
			return _mapping.Map<BookingDto>(booking);
		}

		public async Task<List<Booking>> GetAllPendingStatusBookingAsync()
		{
			return await _bookingRepository.GetBookingsByStatus(BookingStatus.Pending);
		}

		public async Task ConfirmBookingAsync(Guid id)
		{
			await _bookingRepository.UpdateBooking(id, BookingStatus.Confirmed);
		}

		public async Task RejectedBookingAsync(Guid bookingId, Guid? eventId = default)
		{
			await _bookingRepository.UpdateBooking(bookingId, BookingStatus.Rejected);
			if (eventId != null)
				await _eventService.ReleaseSeats(eventId.Value);
		}

		public async Task<bool> CanceledBookingAsync(Guid bookingId)
		{
			var booking = await _bookingRepository.GetBookingById(bookingId);
			if (booking != null)
			{
				var roleString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
				var role = Enum.Parse<Roles>(roleString);
				var idUser = Guid.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

				if (role == Roles.Admin || (role == Roles.User && booking.UserId == idUser))
				{
					await _bookingRepository.UpdateBooking(bookingId, BookingStatus.Cancelled);
					await _eventService.ReleaseSeats(booking.EventId);
				}
				else
					throw new AccessDeniedException();
			}
			else
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return false;
			}
			return true;
		}
	}

#pragma warning restore CS1591
}
