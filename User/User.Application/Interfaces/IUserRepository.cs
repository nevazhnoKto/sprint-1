using User.Domain.Models;

namespace User.Application.Interfaces
{
	/// <summary>
	/// Репозиторий для управления событиями.
	/// </summary>
	public interface IUserRepository
	{
		/// <summary>
		/// Возвращает список всех пользователей.
		/// </summary>
		Task<List<UserModel>> GetUsers();

		/// <summary>
		/// Возвращает пользователя по его уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор пользователя (Guid).</param>
		Task<UserModel?> GetUserByLogin(string login);

		/// <summary>
		/// Добавляет нового пользователей в репозиторий.
		/// </summary>
		/// <param name="newUser">Объект пользователя для добавления.</param>
		Task AddUser(UserModel newUser);

		/// <summary>
		/// Обновляет существующего пользователя.
		/// </summary>
		/// <param name="index">Целочисленный индекс события.</param>
		/// <param name="userCustom">Объект пользователя с новыми данными.</param>
		Task<bool> UpdateUserByIndex(Guid index, UserModel userCustom);

		/// <summary>
		/// Удаляет пользователя по уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор пользователя (Guid).</param>
		Task DeleteUserById(Guid id);
	}
}
