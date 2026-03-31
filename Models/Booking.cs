using WebApiTamakulov.Enums;

namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Модель бронирования.
	/// </summary>
	public class Booking
	{
		/// <summary>
		/// Конструктор для создания новой брони
		/// </summary>
		public Booking(Guid eventId)
		{
			Id = Guid.NewGuid();
			EventId = eventId;
			Status = BookingStatus.Pending;
			CreatedAt = DateTime.UtcNow;
			ProcessedAt = null;
		}

		/// <summary>
		/// Уникальный идентификатор брони.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Идентификатор события, к которому относится бронь.
		/// </summary>
		public Guid EventId { get; set; }
		/// <summary>
		/// Текущий статус брони.
		/// </summary>
		public BookingStatus Status { get; set; }

		/// <summary>
		/// Дата и время создания брони.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Дата и время обработки брони.
		/// </summary>
		public DateTime? ProcessedAt  { get; set; }
	}
}
