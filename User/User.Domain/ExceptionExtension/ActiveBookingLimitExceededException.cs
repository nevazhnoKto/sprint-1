namespace User.Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при достижения лимита бронирования
	/// </summary>
	public class ActiveBookingLimitExceededException : Exception
	{
		/// <summary>
		/// Исключение при достижения лимита бронирования.
		/// </summary>
		public ActiveBookingLimitExceededException(int limit)
	   : base($"Достигнут лимит активных броней. Максимум — {limit}.")
		{
		}

		public ActiveBookingLimitExceededException() : base() { }
	}
}
