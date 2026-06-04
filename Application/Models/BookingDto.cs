using Domain.Enums;

namespace Application.Models
{
	/// <summary>
	/// Модель бронирования.
	/// </summary>
	public class BookingDto
	{
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
	}
}
