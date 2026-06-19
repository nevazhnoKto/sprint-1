using Application.Interfaces;
using Application.Models;

using Infrastructure.SecurityServices;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Net;

namespace Presentation.Controllers
{
	/// <summary>
	/// Api контроллер для работы с регистрацией.
	/// </summary>
	[ApiController]
	[Route("auth")]
	public class AuthController : ControllerBase
	{
		private readonly IUserService _userService;

		/// <summary>
		/// Api контроллер регистрации.
		/// </summary>
		/// <param name="userService">Сервис для работы с регистрацией.</param>
		public AuthController(IUserService userService)
		{
			_userService = userService;
		}

		/// <summary>
		/// Регистрация пользователя.
		/// </summary>
		/// <param name="request">Информация для регистрации.</param>
		/// <returns></returns>
		[HttpPost("/auth/register")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
		public async Task<IActionResult> RegistrationUser([FromBody] RegistrationRequestDto request)
		{
			// Проверка на пустой логин
			if (string.IsNullOrWhiteSpace(request.Login))
			{
				return BadRequest(new ApiResult()
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Логин не может быть пустым"
				});
			}

			// Проверка на пустой пароль
			if (string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest(new ApiResult()
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Пароль не может быть пустым"
				});
			}

			var accessToken = await _userService.RegistrationUser(request.Login, request.Password, request.Role);

			return NoContent();
		}

		/// <summary>
		/// Логирование пользователя.
		/// </summary>
		/// <param name="request">Информация для логирования.</param>
		/// <returns></returns>
		[HttpPost("/auth/login")]
		[AllowAnonymous]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
		{
			// Проверка на пустой логин
			if (string.IsNullOrWhiteSpace(request.Login))
			{
				return BadRequest(new ApiResult()
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Логин не может быть пустым"
				});
			}

			// Проверка на пустой пароль
			if (string.IsNullOrWhiteSpace(request.Password))
			{
				return BadRequest(new ApiResult()
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = $"Пароль не может быть пустым"
				});
			}

			var accessToken = await _userService.LoginUser(request.Login, request.Password);

			return Ok(new { Token = accessToken });
		}
	}
}
