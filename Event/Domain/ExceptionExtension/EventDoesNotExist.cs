namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при недостаточном мест для бронирования.
	/// </summary>
	public class EventDoesNotExist : Exception
	{
		/// <summary>
		/// Исключение при недостаточном мест для бронирования.
		/// </summary>
		public EventDoesNotExist(string eventId)
	   : base($"Событие c EventId {eventId} не существует!")
		{
		}

		public EventDoesNotExist(): base() {}
	}
}
