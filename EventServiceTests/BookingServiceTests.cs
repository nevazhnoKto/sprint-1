using Microsoft.Extensions.Logging;
using Moq;
using OpenQA.Selenium.BiDi.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;
using WebApiTamakulov.Services;

namespace EventServiceTests
{
	public class BookingServiceTests
	{
		private Guid defaultEventGuid = new Guid("00000000-0000-0000-0000-000000000001");
		private readonly IBookingService _bookingService;
		private readonly IBookingRepository _bookingRepository;
		private readonly Mock<IEventService> _eventServiceMock;
		private bool IsEventDeleted = false;

		public BookingServiceTests() 
		{
			var loggerBookingMock = new Mock<ILogger<BookingService>>();
			_eventServiceMock = new Mock<IEventService>();

			Event defaultEvent = new Event()
			{
				Id = defaultEventGuid,
			};
			_eventServiceMock.Setup(m => m.GetById(defaultEventGuid))
							.Returns(() => IsEventDeleted ? null : defaultEvent);

			_eventServiceMock.Setup(m => m.Delete(defaultEventGuid))
							.Returns(true)
							.Callback(() => IsEventDeleted = true);
			_bookingRepository = new BookingRepository();
			_bookingService = new BookingService(loggerBookingMock.Object, _eventServiceMock.Object, _bookingRepository);
			_bookingRepository.Reset();
		}

		[Fact]
		public async Task CreateBooking_ValidBooking_ReturnsTrue()
		{
			//Arrange
			var status = WebApiTamakulov.Enums.BookingStatus.Pending;

			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			//Assert
			Assert.Equal(status, firstBooking.Status);
		}

		[Fact]
		public async Task CreateSomeBooking_ValidBooking_ReturnsTrue()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var secondBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			//Assert
			Assert.NotEqual(firstBooking.Id, secondBooking.Id);
		}

		[Fact]
		public async Task GetBooking_ValidBookingId_ReturnsTrue()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var result = await _bookingService.GetBookingByIdAsync(firstBooking.Id);

			//Assert
			Assert.Equal(defaultEventGuid, result.EventId);
		}

		[Theory]
		[InlineData(WebApiTamakulov.Enums.BookingStatus.Confirmed)]
		public async Task ConfirmStatusBooking_ValidStatus_ReturnsTrue(WebApiTamakulov.Enums.BookingStatus status)
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.ConfirmBookingAsync(firstBooking.Id);
			var result = await _bookingService.GetBookingByIdAsync(firstBooking.Id);

			//Assert
			Assert.Equal(status, result.Status);
		}

		[Theory]
		[InlineData(WebApiTamakulov.Enums.BookingStatus.Rejected)]
		public async Task RejectedStatusBooking_ValidStatus_ReturnsTrue(WebApiTamakulov.Enums.BookingStatus status)
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.RejectedBookingAsync(firstBooking.Id);
			var result = await _bookingService.GetBookingByIdAsync(firstBooking.Id);

			//Assert
			Assert.Equal(status, result.Status);
		}

		[Fact]
		public async Task CreateBooking_NoValidEventId_ReturnsNull()
		{
			
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(Guid.NewGuid());

			//Assert
			Assert.Null(firstBooking);
		}

		[Fact]
		public async Task CreateBooking_DeletedEventId_ReturnsNull()
		{
			// Act
			var booking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			// Удаляем событие
			_eventServiceMock.Object.Delete(defaultEventGuid);

			var bookingAfterDeletedEvent = await _bookingService.GetBookingByIdAsync(booking.Id);

			// Assert
			Assert.NotNull(booking);
			Assert.Null(bookingAfterDeletedEvent);
		}

		[Fact]
		public async Task GetBooking_NoValidBookingId_ReturnsNull()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var result = await _bookingService.GetBookingByIdAsync(Guid.NewGuid());

			//Assert
			Assert.Null(result);
		}

		[Fact]
		public async Task CreateBooking_ValidBooking_ReturnsAvailableSeatsDec()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var result = await _bookingService.GetBookingByIdAsync(Guid.NewGuid());

			//Assert
			Assert.Null(result);
		}
	}
}
