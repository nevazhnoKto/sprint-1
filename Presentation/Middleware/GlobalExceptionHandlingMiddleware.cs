using System.ComponentModel.DataAnnotations;
using Domain.ExceptionExtension;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;

namespace Presentation.Middleware
{

	/// <summary>
	/// Middleware-обработчик ошибок.
	/// </summary>
	public class GlobalExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

		/// <summary>
		/// Middleware-обработчик ошибок.
		/// </summary>
		public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		/// <summary>
		/// Выполняет middleware для обработки HTTP-запроса.
		/// </summary>
		/// <param name="httpContext"></param>
		/// <returns></returns>
		public async Task InvokeAsync(HttpContext httpContext)
		{
			try
			{
				await _next(httpContext);
			}
			catch (Exception ex)
			{
				await HandleException(httpContext, ex);
			}
		}

		private async Task HandleException(HttpContext httpContext, Exception ex)
		{
			_logger.LogError(
				ex,
				"Unhandled exception. Method={Method}, Path={Path}, RequestId={RequestId}",
				httpContext.Request.Method,
				httpContext.Request.Path,
				httpContext.Request.Headers["x-request-id"]);

			if (httpContext.Response.HasStarted)
			{
				return;
			}

			var statusCode = StatusCodeMapping(ex);

			httpContext.Response.StatusCode = statusCode;
			httpContext.Response.ContentType = "application/json";

			var error = new ProblemDetails
			{
				Status = statusCode,
				Detail = ex.Message
			};

			await httpContext.Response.WriteAsJsonAsync(error);
		}

		private static int StatusCodeMapping(Exception ex)
		=> ex switch
		{
			ValidationException => StatusCodes.Status400BadRequest,
			EventDoesNotExist => StatusCodes.Status404NotFound,
			NoAvailableSeatsException => StatusCodes.Status409Conflict,
			_ => StatusCodes.Status500InternalServerError
		};
	}
}
