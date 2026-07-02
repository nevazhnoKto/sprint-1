using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Application.Interfaces;
using User.Domain.Models;

namespace User.Infrastructure.SecurityServices
{
	public class TokenGenerator : ITokenGenerator
	{
		private readonly JwtSettings _settings;

		public TokenGenerator(IOptions<JwtSettings> settings)
		{
			_settings = settings.Value;
		}

		public string GenerateToken(UserModel user)
		{
			var claims = new List<Claim>
			{
				new (ClaimTypes.Name, user.Login),
				new (ClaimTypes.NameIdentifier, user.Id.ToString()),
				new (ClaimTypes.Role, user.Role.ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _settings.Issuer,
				audience: _settings.Audience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(_settings.ExpiryMinutes),
				signingCredentials: creds
				);

			// Запись в строку и отправка клиенту
			string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

			return accessToken;
		}
	}
}
