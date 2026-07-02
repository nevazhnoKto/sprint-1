namespace SharedContract
{
	/// <summary>
	/// Константы для интеграции, связанные с бронированием.
	/// </summary>
	public static class BookingTopics
	{
		/// <summary>
		/// Имя топика/очереди для подтвержденных бронирований.
		/// </summary>
		public const string BookingConfirmed = "booking-confirmed";

		/// <summary>
		/// Имя топика/очереди для отмененных бронирований.
		/// </summary>
		public const string BookingCanceled = "booking-canceled";
	}

	/// <summary>
	/// Неизменяемый контракт события подтверждения бронирования.
	/// Содержит только публичные данные, необходимые сервисам-подписчикам.
	/// </summary>
	public record BookingConfirmedEvent(
		Guid BookingId,
		Guid EventId,
		Guid UserId);

	/// <summary>
	/// Неизменяемый контракт события отмены бронирования.
	/// </summary>
	public record BookingCanceledEvent(
		Guid BookingId,
		Guid EventId,
		Guid UserId,
		string Reason);
}