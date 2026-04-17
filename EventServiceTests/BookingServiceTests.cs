using Microsoft.Extensions.Logging;
using Moq;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Session;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTamakulov.Enums;
using WebApiTamakulov.ExceptionExtension;
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

			Event defaultEvent = new Event(defaultEventGuid, "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10);
			
			_eventServiceMock.Setup(m => m.GetById(defaultEventGuid))
							.Returns(() => IsEventDeleted ? null : defaultEvent);

			_eventServiceMock.Setup(m => m.Delete(defaultEventGuid))
							.Returns(true)
							.Callback(() => IsEventDeleted = true);

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>())).Returns(true);
			_bookingRepository = new BookingRepository();
			_bookingService = new BookingService(loggerBookingMock.Object, _eventServiceMock.Object, _bookingRepository);
			_bookingRepository.Reset();
		}

		[Fact]
		public async Task CreateBooking_ValidBooking_ReturnsTrue()
		{
			//Arrange
			var status = BookingStatus.Pending;

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
		[InlineData(BookingStatus.Confirmed)]
		public async Task ConfirmStatusBooking_ValidStatus_ReturnsTrue(BookingStatus status)
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.ConfirmBookingAsync(firstBooking.Id);
			var result = await _bookingService.GetBookingByIdAsync(firstBooking.Id);

			//Assert
			Assert.Equal(status, result.Status);
		}

		[Theory]
		[InlineData(BookingStatus.Rejected)]
		public async Task RejectedStatusBooking_ValidStatus_ReturnsTrue(BookingStatus status)
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.RejectedBookingAsync(firstBooking.Id);
			var result = await _bookingService.GetBookingByIdAsync(firstBooking.Id);

			//Assert
			Assert.Equal(status, result.Status);
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
		public async Task CreateBooking_ValidBooking_ReturnsNotNull()
		{
			// Arrange
			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>())).Returns(true);

			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			//Assert
			Assert.NotNull(firstBooking);
		}

		[Fact]
		public async Task CreateManyBooking_ValidBookings_ReturnsNotNulls()
		{
			// Arrange
			var callCount = 0;

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.Returns(() =>
				{
					callCount++;
					return callCount <= 3;
				});

			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var secondBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var thirdBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			//Assert
			Assert.NotNull(firstBooking);
			Assert.NotNull(secondBooking);
			Assert.NotNull(thirdBooking);
		}

		[Fact]
		public async Task CreateManyBooking_NoEvalibleCount_ReturnsNotNulls()
		{
			// Arrange
			var callCount = 0;

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.Returns(() =>
				{
					callCount++;
					return callCount <= 3;
				});

			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var secondBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			var thirdBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);

			//Assert
			Assert.NotNull(firstBooking);
			Assert.NotNull(secondBooking);
			Assert.NotNull(thirdBooking);
			await Assert.ThrowsAsync<NoAvailableSeatsException>(() => _bookingService.CreateBookingAsync(defaultEventGuid));
		}

		[Fact]
		public async Task CreateBooking_NotSeats_ReturnsNoAvailableSeatsException()
		{
			// Arrange
			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.Returns(() => false);
			
			//Assert
			await Assert.ThrowsAsync<NoAvailableSeatsException>(() => _bookingService.CreateBookingAsync(defaultEventGuid));
		}

		[Fact]
		public async Task CreateBooking_NotValidEvent_ReturnsNotFoundException()
		{
			// Arrange
			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>())).Throws<NotFoundException>();
			//Assert
			await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.CreateBookingAsync(Guid.NewGuid()));
		}

		[Fact]
		public async Task Confirm_ValidBooking_ReturnsTrue()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.ConfirmBookingAsync(firstBooking.Id);

			//Assert
			Assert.Equal(BookingStatus.Confirmed, firstBooking.Status);
			Assert.NotNull(firstBooking.ProcessedAt);
		}

		[Fact]
		public async Task Rejected_ValidBooking_ReturnsTrue()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			_bookingService.RejectedBookingAsync(firstBooking.Id);

			//Assert
			Assert.Equal(BookingStatus.Rejected, firstBooking.Status);
			Assert.NotNull(firstBooking.ProcessedAt);
		}

		[Fact]
		public async Task UniqId_ValidBooking_ReturnsTrue()
		{
			// Arrange
			var callCount = 0;

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.Returns(() =>
				{
					callCount++;
					return callCount <= 10;
				});
			var concurrentRequests = 10;

			//Act
			var tasks = new Task<Booking>[concurrentRequests];
			for (int i = 0; i < concurrentRequests; i++)
			{
				tasks[i] = _bookingService.CreateBookingAsync(defaultEventGuid);
			}

			// Ждем завершения всех задач
			var results = await Task.WhenAll(tasks);
			
			//Assert
			var distinctId = results.Select(d => d.Id).Distinct().ToList();
			Assert.Equal(10, distinctId.Count);
		}

		[Fact]
		public async Task NoAvailableCount_ValidBooking_ReturnsTrue()
		{
			// Arrange
			var callCount = 0;
			var successfulBookings = 0;
			var exceptionsBookings = 0;

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.Returns(() =>
				{
					callCount++;
					return callCount <= 5;
				});
			var concurrentRequests = 20;

			//Act
			// Act - запускаем параллельно
			await Parallel.ForEachAsync(
				Enumerable.Range(0, concurrentRequests),
				async (_, _) =>
				{
					try
					{
						var booking = await _bookingService.CreateBookingAsync(defaultEventGuid);
						successfulBookings++;
					}
					catch
					{
						exceptionsBookings++;
					}
				});

			//Assert
			Assert.Equal(15, exceptionsBookings);
			Assert.Equal(5, successfulBookings);
		}
	}
}
