using Booking.Application.Interfaces;
using Booking.Application.Models;
using Booking.Domain.Common;
using Booking.Domain.Enums;
using Booking.Domain.ExceptionExtension;
using Booking.Domain.Models;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace Booking.Application.Services
{

#pragma warning disable CS1591

	public class BookingService : IBookingService
	{
		private readonly ILogger<BookingService> _logger;
		private readonly IBookingRepository _bookingRepository;
		private readonly IMapper _mapping;
		private readonly IKafkaIntegration _kafka;

		public BookingService(ILogger<BookingService> logger, IBookingRepository bookingRepository, IMapper mapping, IKafkaIntegration kafka)
		{
			_logger = logger;
			_bookingRepository = bookingRepository;
			_mapping = mapping;
			_kafka = kafka;
		}

		public async Task ConfirmBookingAsync(BookingModel bookingModel)
		{
			await _bookingRepository.UpdateBooking(bookingModel.Id, BookingStatus.Confirmed);
			await _kafka.SendBookingConfirmedKafka(bookingModel);
		}
		public async Task<bool> CanceledBookingAsync(Guid bookingId, Guid userId, string role)
		{
			var booking = await _bookingRepository.GetBookingById(bookingId);

			if (booking != null)
			{
				if (booking.Status == BookingStatus.Cancelled)
				{
					_logger.LogInformation($"Бронирования {bookingId} уже отменено!");
					return false;
				}
				if (IsRoleAdmin(role) || (!IsRoleAdmin(role) && booking.UserId == userId))
				{
					await _bookingRepository.UpdateBooking(bookingId, BookingStatus.Cancelled);
					await _kafka.SendBookingCanceledKafka(booking);
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

		public async Task<BookingDto> CreateBookingAsync(Guid eventId, Guid userId)
		{
			var countBookingByUser = await _bookingRepository.GetCountBookingByUserId(userId);
			if (countBookingByUser >= CommonConst.LimitBookingForUser)
				throw new ActiveBookingLimitExceededException(CommonConst.LimitBookingForUser);

			var newBooking = await _bookingRepository.AddBooking(eventId, userId);

			var message = $"Бронирования для события с eventId = {eventId} созданно!";
			_logger.LogInformation(message);

			return _mapping.Map<BookingDto>(newBooking);
		}

		public async Task<List<BookingModel>> GetAllPendingStatusBookingAsync()
		{
			return await _bookingRepository.GetBookingsByStatus(BookingStatus.Pending);
		}

		public async Task<BookingDto> GetBookingByIdAsync(Guid bookingId, Guid userId, string role)
		{
			var booking = await _bookingRepository.GetBookingById(bookingId);
			if (booking == null)
			{
				_logger.LogInformation($"Бронирования с {bookingId} не существует!");
				return default!;
			}
			// Если бронирование не принадлежит пользователю и он не админ то не выдаем информацию.
			if (!IsRoleAdmin(role) && booking.UserId != userId)
			{
				throw new AccessDeniedException();
			}
			_logger.LogInformation($"Найдено бронирование с id = {bookingId}");
			return _mapping.Map<BookingDto>(booking);
		}

		public async Task RejectedBookingAsync(Guid bookingId, Guid? eventId = default)
		{
			await _bookingRepository.UpdateBooking(bookingId, BookingStatus.Rejected);
		}

		private bool IsRoleAdmin(string role)
		{
			return role == "Admin";
		}
	}

#pragma warning restore CS1591
}
