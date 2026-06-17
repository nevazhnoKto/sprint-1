using Application.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.SecurityServices
{
	public class TokenGenerator : ITokenGenerator
	{
		private readonly JwtSettings _settings;

		public TokenGenerator(IOptions<JwtSettings> settings)
		{
			_settings = settings.Value;
		}

		public string GenerateToken(User user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Login),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Role, user.Role.ToString())
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
