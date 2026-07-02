using Domain.Enums;

namespace Domain.Models
{
	/// <summary>
	/// Модель бронирования.
	/// </summary>
	public class Booking
	{
		/// <summary>
		/// Пустой конструктор для EF Core.
		/// </summary>
		private Booking()
		{
			Id = Guid.NewGuid();
			UserId = Guid.Empty;
			EventId = Guid.Empty;
			Status = BookingStatus.Pending;
			CreatedAt = DateTime.UtcNow;
		}

		/// <summary>
		/// Конструктор для создания новой брони
		/// </summary>
		public Booking(Guid eventId, Guid userId)
		{
			Id = Guid.NewGuid();
			UserId = userId;
			EventId = eventId;
			Status = BookingStatus.Pending;
			CreatedAt = DateTime.UtcNow;
			ProcessedAt = null;
		}

		/// <summary>
		/// Уникальный идентификатор брони.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Идентификатор пользователя, к которому относится бронь.
		/// </summary>
		public Guid UserId { get; set; }

		/// <summary>
		/// Идентификатор события, к которому относится бронь.
		/// </summary>
		public Guid EventId { get; private set; }
		/// <summary>
		/// Текущий статус брони.
		/// </summary>
		public BookingStatus Status { get; set; }

		/// <summary>
		/// Дата и время создания брони.
		/// </summary>
		public DateTime CreatedAt { get; private set; }

		/// <summary>
		/// Дата и время обработки брони.
		/// </summary>
		public DateTime? ProcessedAt  { get; set; }
	}
}
