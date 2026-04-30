using WebApiTamakulov.Enums;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
{
	/// <summary>
	/// Интерфейс репозитория для работы с бронированиями.
	/// </summary>
	public interface IBookingRepository
	{
		/// <summary>
		/// Создаёт новое бронирование для указанного события.
		/// </summary>
		/// <param name="eventId">Идентификатор события.</param>
		Task<Booking> AddBooking(Guid eventId);

		/// <summary>
		/// Возвращает бронирование по его идентификатору.
		/// </summary>
		/// <param name="bookingId">Идентификатор бронирования.</param>
		Task<Booking?> GetBookingById(Guid bookingId);

		/// <summary>
		/// Удаляет бронирование по идентификатору.
		/// </summary>
		/// <param name="bookingId">Идентификатор бронирования.</param>
		Task DeleteBookingById(Guid bookingId);

		/// <summary>
		/// Возвращает список бронирований с указанным статусом.
		/// </summary>
		/// <param name="status">Статус бронирования (например, Active, Cancelled).</param>
		Task<List<Booking>> GetBookingsByStatus(Enums.BookingStatus status);

		/// <summary>
		/// Обновляет статус существующего бронирования.
		/// </summary>
		/// <param name="id">Идентификатор бронирования.</param>
		/// <param name="status">Новый статус бронирования.</param>
		Task UpdateBooking(Guid id, BookingStatus status);

		/*/// <summary>
		/// Костыль для тестов, чтобы сбрасывать статик переменную.
		/// </summary>
		void Reset();*/
	}
}
