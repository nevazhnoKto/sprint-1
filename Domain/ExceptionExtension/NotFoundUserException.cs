namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при отсутствии прав.
	/// </summary>
	public class NotFoundUserException : Exception
	{
		/// <summary>
		/// Исключение при отсутствии прав.
		/// </summary>
		public NotFoundUserException(string message)
	   : base($"Пользователя с логином {message} не существует.")
		{
		}
	}
}
