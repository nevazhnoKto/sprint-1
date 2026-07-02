using Application.Models;
using Domain.Enums;
using Domain.Models;

namespace Application.Interfaces
{
	/// <summary>
	/// Сервис бронирования.
	/// </summary>
	public interface IBookingService
	{
		/// <summary>
		/// Создать бронирование по eventId.
		/// </summary>
		/// <param name="eventId">Id события, для которого создать бронь.</param>
		/// <param name="userId">Id пользователя, для которого создать бронь.</param>
		/// <returns>Созданная бронь.</returns>
		Task<BookingDto> CreateBookingAsync(Guid eventId, Guid userId);

		/// <summary>
		/// Получить бронь по Id брони.
		/// </summary>
		/// <param name="bookingId">Id брони.</param>
		/// <returns>Бронь по Id.</returns>
		Task<BookingDto> GetBookingByIdAsync(Guid bookingId, Guid userId);

		/// <summary>
		/// Получить все брони со статусом "В ожидании".
		/// </summary>
		/// <returns>Список всех броней со статусом "В ожидании".</returns>
		Task<List<Booking>> GetAllPendingStatusBookingAsync();

		/// <summary>
		/// Подтверждение бронирования.
		/// </summary>
		/// <param name="id">Id бронирования.</param>
		Task ConfirmBookingAsync(Guid id);

		/// <summary>
		/// Отклоненение бронирования.
		/// </summary>
		/// <param name="bookingId">Id бронирования.</param>
		/// <param name="eventId">Id события.</param>
		Task RejectedBookingAsync(Guid bookingId, Guid? eventId = default);

		/// <summary>
		/// Отмена бронирования.
		/// </summary>
		/// <param name="bookingId">Id бронирования.</param>
		Task<bool> CanceledBookingAsync(Guid bookingId, Guid userId);
	}
}
