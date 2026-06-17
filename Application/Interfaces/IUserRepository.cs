using Domain.Models;

namespace Application.Interfaces
{
	/// <summary>
	/// Репозиторий для управления событиями.
	/// </summary>
	public interface IUserRepository
	{
		/// <summary>
		/// Возвращает список всех пользователей.
		/// </summary>
		Task<List<User>> GetUsers();

		/// <summary>
		/// Возвращает пользователя по его уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор пользователя (Guid).</param>
		Task<User?> GetUserById(Guid id);

		/// <summary>
		/// Добавляет нового пользователей в репозиторий.
		/// </summary>
		/// <param name="newUser">Объект пользователя для добавления.</param>
		Task AddUser(User newUser);

		/// <summary>
		/// Обновляет существующего пользователя.
		/// </summary>
		/// <param name="index">Целочисленный индекс события.</param>
		/// <param name="userCustom">Объект пользователя с новыми данными.</param>
		Task<bool> UpdateUserByIndex(Guid index, User userCustom);

		/// <summary>
		/// Удаляет пользователя по уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор пользователя (Guid).</param>
		Task DeleteUserById(Guid id);
	}
}
