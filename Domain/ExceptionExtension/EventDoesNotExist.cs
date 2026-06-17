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
		public EventDoesNotExist(string message)
	   : base("message")
		{
		}

		public EventDoesNotExist(): base() {}
	}
}
