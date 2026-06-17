using Domain.Common;

namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при достижения лимита бронирования
	/// </summary>
	public class ActiveBookingLimitExceededException : Exception
	{
		/// <summary>
		/// Исключение при достижения лимита бронирования.
		/// </summary>
		public ActiveBookingLimitExceededException()
	   : base($"Достигнут лимит активных броней. Максимум — {CommonConst.LimitBookingForUser}.")
		{
		}
	}
}
