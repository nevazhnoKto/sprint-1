using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
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
		/// <returns>Созданная бронь.</returns>
		Task<Booking> CreateBookingAsync(Guid eventId);

		/// <summary>
		/// Получить бронь по Id брони.
		/// </summary>
		/// <param name="bookingId">Id брони.</param>
		/// <returns>Бронь по Id.</returns>
		Task<Booking> GetBookingByIdAsync(Guid bookingId);

		/// <summary>
		/// Получить все брони со статусом "В ожидании".
		/// </summary>
		/// <returns>Список всех броней со статусом "В ожидании".</returns>
		List<Booking> GetAllPendingStatusBookingAsync();

		void ConfirmBookingAsync(Guid id);

		void RejectedBookingAsync(Guid bookingId, Guid? eventId = default);
	}
}
