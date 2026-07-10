namespace Booking.Domain.ExceptionExtension
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
	   : base("Недостаточном мест для бронирования.")
		{
		}
	}
}
