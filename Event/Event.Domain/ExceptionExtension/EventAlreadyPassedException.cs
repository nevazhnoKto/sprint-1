namespace Event.Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение бронировании завершенного события.
	/// </summary>
	public class EventAlreadyPassedException : Exception
	{
		/// <summary>
		/// Исключение бронировании завершенного события.
		/// </summary>
		public EventAlreadyPassedException(string eventId)
	   : base($"Невозможно забронировать билет, так как событие {eventId} уже началось.")
		{
		}

		public EventAlreadyPassedException() : base() { }
	}
}
