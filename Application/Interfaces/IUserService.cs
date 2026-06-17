using Application.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
	public interface IUserService
	{
		Task<string> RegistrationUser(string login, string password, Roles role);
		Task<string> LoginUser(string login, string password);
	}
}
