using System.ComponentModel.DataAnnotations;
using Booking.Domain.ExceptionExtension;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Presentation.Middleware
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
			AccessDeniedException => StatusCodes.Status403Forbidden,
			EventAlreadyPassedException or ValidationException => StatusCodes.Status400BadRequest,
			ActiveBookingLimitExceededException or NoAvailableSeatsException or DuplicateLoginException => StatusCodes.Status409Conflict,
			EventDoesNotExist or UnauthorizedAccessException => StatusCodes.Status404NotFound,
			_ => StatusCodes.Status500InternalServerError
		};
	}
}
