using User.Domain.Enums;

namespace User.Application.Interfaces
{
	public interface IUserService
	{
		Task<string> RegistrationUser(string login, string password, Roles role);
		Task<string> LoginUser(string login, string password);
	}
}
