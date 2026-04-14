using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Controllers
{
	/// <summary>
	/// Api контроллер для работы с Событиями.
	/// </summary>
	[ApiController]
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
		[ProducesResponseType(typeof(ApiResult), StatusCodes.Status404NotFound)]
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
			if (booking != null)
			{
				var response = new ApiResult<BookingDto>
				{
					Success = true,
					Data = _mapper.Map<BookingDto>(booking),
					StatusCode = HttpStatusCode.Accepted,
					Message = $"Создалось бронирование для EventId = {id}. BookingId = {booking.Id}"
				};
				return AcceptedAtAction(nameof(GetByIdBooking), new { id = booking.Id }, response);
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Event по id = {id} не существует, невозможно создать событие!"
			});
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
					Message = $"Вернул статус бронирования по id = {id}"
				});
			}

			return NotFound(new ApiResult()
			{
				Success = false,
				StatusCode = HttpStatusCode.NotFound,
				Message = $"Бронирование по id = {id} не существует"
			});
		}

	}
}
