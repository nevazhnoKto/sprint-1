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
	/// Api контроллер для работы с Событиями.
	/// </summary>
	[ApiController]
	[Route("users")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		/// <summary>
		/// Api контроллер для работы с Событиями.
		/// </summary>
		/// <param name="eventService">Сервис для работы с Событиями.</param>
		/// <param name="mapper">Маппер.</param>
		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("registration")]
		[AllowAnonymous]
		public IActionResult RegistrationUser([FromBody] RegistrationRequestDto request)
		{
			var accessToken = _userService.RegistrationUser(request.Login, request.Password, request.Role);

			// 4. Вернуть ответ
			return Ok(new { Token = accessToken });
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public IActionResult Login([FromBody] LoginRequestDto request)
		{
			var accessToken = _userService.LoginUser (request.Login, request.Password);

			return Ok(new { Token = accessToken });
		}
	}
}
