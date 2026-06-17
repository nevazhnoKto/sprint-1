using Application.Interfaces;
using Domain.Enums;
using Domain.ExceptionExtension;
using Domain.Models;

namespace Application.Services
{
	internal class UserService : IUserService
	{
		private readonly ITokenGenerator _tokenGenerator;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IUserRepository _userRepository;

		public UserService(ITokenGenerator tokenGenerator, IPasswordHasher passwordHasher, IUserRepository userRepository)
		{
			_tokenGenerator = tokenGenerator;
			_passwordHasher = passwordHasher;
			_userRepository = userRepository;
		}
		public async Task<string> RegistrationUser(string login, string password, Roles role)
		{
			var user = await _userRepository.GetUserByLogin(login);
			if (user != null)
				throw new DuplicateLoginException(login);

			var hashPassword = _passwordHasher.HashPassword(password);
			user = new User(login, hashPassword, role);
			var token = _tokenGenerator.GenerateToken(user);
			await _userRepository.AddUser(user);
			return token;
		}

		public async Task<string> LoginUser(string login, string password)
		{
			var user = await _userRepository.GetUserByLogin(login);
			if (user == null)
				throw new NotFoundUserException(login);

			// Проверить пароль
			if (!_passwordHasher.VerifyPassword(user.HashPassword, password))
				throw new UnauthorizedAccessException("Invalid login or password");


			// 3. Сгенерировать токен
			var token = _tokenGenerator.GenerateToken(user);
			return token;
		}

		
	}
}
