using Domain.Enums;

namespace Domain.Models
{
	/// <summary>
	/// Модель бронирования.
	/// </summary>
	public class User
	{
		/// <summary>
		/// Пустой конструктор для EF Core.
		/// </summary>
		private User() 
		{
			Login = string.Empty;
			HashPassword = string.Empty;
		}

		/// <summary>
		/// Конструктор для создания нового пользователя
		/// </summary>
		public User(string login, string hashPassword, Roles role)
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

		/// <summary>
		/// Коллекция Bookings.
		/// </summary>
		public List<Booking>? Bookings { get; set; }
	}
}
