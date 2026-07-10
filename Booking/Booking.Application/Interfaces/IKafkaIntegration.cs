using Booking.Domain.Models;
using Booking.Domain.Enums;

namespace Booking.Application.Interfaces
{
	/// <summary>
	/// Интерфейс отправки сообщений в кафку.
	/// </summary>
	public interface IKafkaIntegration
	{
		/// <summary>
		/// Отправляет сообщение о подтверждении в кафку.
		/// </summary>
		/// <param name="eventId">Модель букинга.</param>
		Task SendBookingConfirmedKafka(BookingModel bookingModel);

		/// <summary>
		/// Отправляет сообщение об отмене брони в кафку.
		/// </summary>
		/// <param name="eventId">Модель букинга.</param>
		Task SendBookingCanceledKafka(BookingModel bookingModel);
	}
}
