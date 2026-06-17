namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение бронировании завершенного события.
	/// </summary>
	public class EventAlreadyPassedException : Exception
	{
		/// <summary>
		/// Исключение бронировании завершенного события.
		/// </summary>
		public EventAlreadyPassedException()
	   : base("Невозможно забронировать билет, так как событие уже завершено.")
		{
		}
	}
}
