namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при отсутствии прав.
	/// </summary>
	public class DuplicateLoginException : Exception
	{
		public DuplicateLoginException() : base() { }

		/// <summary>
		/// Исключение при отсутствии прав.
		/// </summary>
		public DuplicateLoginException(string message)
	   : base($"Пользователь с логином {message} уже существует.")
		{
		}
	}
}
