using System.Security.Cryptography;
using System.Text;
using User.Application.Interfaces;

namespace User.Infrastructure.SecurityServices
{
	public class PasswordHasher : IPasswordHasher
	{
		public string HashPassword(string password)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
			return Convert.ToHexString(bytes);
		}

		public bool VerifyPassword(string hashedPassword, string plainPassword)
		{
			// 1. Хэшируем введенный пароль тем же методом
			var hashOfInput = HashPassword(plainPassword);

			// 2. Сравниваем строки (без учета регистра, так как Hex в верхнем регистре)
			return string.Equals(hashedPassword, hashOfInput, StringComparison.OrdinalIgnoreCase);
		}
	}
}
