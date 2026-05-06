namespace WebApiTamakulov.ExceptionExtension
{
	/// <summary>
	/// Исключение при недостаточном мест для бронирования.
	/// </summary>
	public class NoAvailableSeatsException : Exception
	{
		/// <summary>
		/// Исключение при недостаточном мест для бронирования.
		/// </summary>
		public NoAvailableSeatsException()
	   : base("No available seats for this event")
		{
		}
	}
}
