using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Domain.Models
{
	public class InboxMessage
	{
		/// <summary>
		/// Уникальный идентификатор сообщения (сюда будем передавать BookingId).
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
		/// Имя типа события для логирования и аналитики.
		/// </summary>
		public string MessageName { get; set; } = string.Empty;

		/// <summary>
		/// Дата и время обработки сообщения.
		/// </summary>
		public DateTimeOffset ProcessedAt { get; set; }
	}
}
