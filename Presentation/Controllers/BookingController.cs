using Application.Interfaces;
using Application.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using System.Net;
using System.Security.Claims;

namespace Presentation.Controllers
{
	/// <summary>
	/// Api контроллер для работы с Событиями.
	/// </summary>
	[ApiController]
	[Authorize]
	[Route("events")]
	public class BookingController : ControllerBase
	{
		private readonly IBookingService _bookingService;
		private readonly IMapper _mapper;
		/// <summary>
		/// Api контроллер для работы с Событиями.
		/// </summary>
		/// <param name="mapper">Маппер.</param>
		/// <param name="bookingService">Сервис для работы с бронированием.</param>
		public BookingController(IMapper mapper, IBookingService bookingService)
		{
			_mapper = mapper;
			_bookingService = bookingService;
		}

		/// <summary>
		/// Метод создает новое бронирования для конкретного EventId.
		/// </summary>
		/// <param name="id">Id события для бронирования.</param>
		[HttpPost("{id:Guid}/book")]
		[ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status202Accepted)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
		[Produces("application/json")]
		public async Task<IActionResult> CreateBooking(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest(new ApiResult
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = "EventId не может быть пустым!"
				});
			}

			var booking = await _bookingService.CreateBookingAsync(id);

			var response = new ApiResult<BookingDto>
			{
				Success = true,
				Data = booking,
				StatusCode = HttpStatusCode.Accepted,
				Message = $"Создалось бронирование для EventId = {id}. BookingId = {booking.Id}. Статус {booking.Status}"
			};
			return AcceptedAtAction(nameof(GetByIdBooking), new { id = booking.Id }, response);
		}

		/// <summary>
		/// Метод возвращает бронирование по запрашиваемому Id.
		/// </summary>
		/// <param name="id">Запрашиваемый Id бронирования.</param>
		[HttpGet("/bookings/{id:Guid}")]
		[ProducesResponseType(typeof(ApiResult<BookingDto>), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
		[Produces("application/json")]
		public async Task<IActionResult> GetByIdBooking(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest(new ApiResult
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = "BookingID не может быть пустым!"
				});
			}

			var bookingById = await _bookingService.GetBookingByIdAsync(id);

			if (bookingById != null)
			{
				return Ok(new ApiResult<BookingDto>()
				{
					Success = true,
					Data = _mapper.Map<BookingDto>(bookingById),
					StatusCode = HttpStatusCode.OK,
					Message = $"Вернул статус ({bookingById.Status}) бронирования по id = {id}"
				});
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Бронирование по id = {id} не существует"
			});
		}

		/// <summary>
		/// Метод создает новое бронирования для конкретного EventId.
		/// </summary>
		/// <param name="id">Id события для бронирования.</param>
		[HttpPut("/CancelBookings/{id:Guid}")]
		[Produces("application/json")]
		public async Task<IActionResult> CancelBooking(Guid id)
		{
			if (id == Guid.Empty)
			{
				return BadRequest(new ApiResult
				{
					Success = false,
					StatusCode = HttpStatusCode.BadRequest,
					Message = "Booking не может быть пустым!"
				});
			}

			var result = await _bookingService.CanceledBookingAsync(id);

			if (!result)
			{
				return NotFound(new ApiResult
				{
					Success = false,
					StatusCode = HttpStatusCode.NotFound,
					Message = $"Бронирование с ID {id} не найдено"
				});
			}

			var response = new ApiResult
			{
				Success = true,
				StatusCode = HttpStatusCode.OK,
				Message = $"Бронирование {id} успешно отменено"
			};

			return Ok(response);
		}

	}
}
