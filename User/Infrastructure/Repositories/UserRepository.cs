using Microsoft.EntityFrameworkCore;
using User.Application.Interfaces;
using User.Domain.Models;
using User.Infrastructure.DataAccess;

namespace User.Infrastructure.Repositories
{
#pragma warning disable CS1591
	public class UserRepository : IUserRepository
	{
		private readonly AppDbContext _context;

		public UserRepository(AppDbContext context)
		{
			_context = context;
		}
		public async Task AddUser(UserModel newUser)
		{
				_context.Users.Add(newUser);
				await _context.SaveChangesAsync();
		}

		public async Task DeleteUserById(Guid id)
		{
			var findUser = _context.Users.FirstOrDefault(e => e.Id == id);
			if (findUser != null)
			{
				_context.Users.Remove(findUser);
				await _context.SaveChangesAsync();
			}
		}

		public async Task<UserModel?> GetUserByLogin(string login)
		{
			return await _context.Users.FirstOrDefaultAsync(e => e.Login == login);
		}

		public async Task<List<UserModel>> GetUsers()
		{
			return await _context.Users.ToListAsync();
		}

		public async Task<bool> UpdateUserByIndex(Guid index, UserModel userCustom)
		{
			var existingUser = _context.Users.FirstOrDefault(e => e.Id == index);
			
			if (existingUser != null)
			{
				// Обновляем только нужные поля
				existingUser.Role = userCustom.Role;
				existingUser.HashPassword = userCustom.HashPassword;
				existingUser.Login = userCustom.Login;
				await _context.SaveChangesAsync();
				return true;
			}
			return false;
		}
	}
#pragma warning restore CS1591
}
