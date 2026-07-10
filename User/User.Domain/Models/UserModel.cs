using User.Domain.Enums;

namespace User.Domain.Models
{
	/// <summary>
	/// Модель бронирования.
	/// </summary>
	public class UserModel
	{
		/// <summary>
		/// Пустой конструктор для EF Core.
		/// </summary>
		private UserModel() 
		{
			Login = string.Empty;
			HashPassword = string.Empty;
		}

		/// <summary>
		/// Конструктор для создания нового пользователя
		/// </summary>
		public UserModel(string login, string hashPassword, Roles role)
		{
			Id = Guid.NewGuid();
			Login = login;
			Role = role;
			HashPassword = hashPassword;
		}

		/// <summary>
		/// Уникальный идентификатор пользователя.
		/// </summary>
		public Guid Id { get; private set; }

		/// <summary>
		/// Логин пользователя.
		/// </summary>
		public string Login { get; set; }

		/// <summary>
		/// Хэш пароль пользователя.
		/// </summary>
		public string HashPassword { get; set; }

		/// <summary>
		/// Роль пользователя.
		/// </summary>
		public Roles Role{ get; set; }
	}
}
