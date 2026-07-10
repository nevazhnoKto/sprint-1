namespace Booking.Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при отсутствии прав.
	/// </summary>
	public class DuplicateLoginException : Exception
	{
		/// <summary>
		/// Исключение при отсутствии прав.
		/// </summary>
		public DuplicateLoginException(string message)
	   : base($"Пользователь с логином {message} уже существует.")
		{
		}

		public DuplicateLoginException() : base() { }
	}
}
