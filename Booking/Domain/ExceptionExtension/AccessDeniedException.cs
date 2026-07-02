namespace Domain.ExceptionExtension
{
	/// <summary>
	/// Исключение при отсутствии прав.
	/// </summary>
	public class AccessDeniedException : Exception
	{
		/// <summary>
		/// Исключение при отсутствии прав.
		/// </summary>
		public AccessDeniedException()
	   : base($"У вас недостаточно прав для выполнения этой операции.")
		{
		}
	}
}
