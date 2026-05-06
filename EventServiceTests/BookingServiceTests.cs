using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OpenQA.Selenium;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Enums;
using WebApiTamakulov.ExceptionExtension;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;
using WebApiTamakulov.Repositories;
using WebApiTamakulov.Services;

namespace EventServiceTests
{
	public class BookingServiceTests : IDisposable
	{
		private Guid defaultEventGuid = new Guid("00000000-0000-0000-0000-000000000001");
		private readonly IBookingService _bookingService;
		private readonly Mock<IEventService> _eventServiceMock;
		private readonly ServiceProvider _serviceProvider;
		private readonly AppDbContext _context;

		private bool IsEventDeleted = false;

		/// <summary>
		/// Конструктор тестов.
		/// </summary>
		public BookingServiceTests() 
		{
			var services = new ServiceCollection();

			var dbName = Guid.NewGuid().ToString();
			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));

			services.AddScoped<IBookingRepository, WebApiTamakulov.Repositories.BookingRepository>();

			_eventServiceMock = new Mock<IEventService>();
			services.AddScoped(_ => _eventServiceMock.Object);

			services.AddScoped<IBookingService, BookingService>();

			var loggerMock = new Mock<ILogger<BookingService>>();
			services.AddScoped(_ => loggerMock.Object);

			_serviceProvider = services.BuildServiceProvider();

			_context = _serviceProvider.GetRequiredService<AppDbContext>();

			_bookingService = _serviceProvider.GetRequiredService<IBookingService>();

			Event defaultEvent = new Event(defaultEventGuid, "Первое событие", "Очень классное событие",
				DateTime.Now, DateTime.Now.AddHours(2), 10);

			_eventServiceMock.Setup(m => m.GetById(defaultEventGuid))
				.ReturnsAsync(() => IsEventDeleted ? null : defaultEvent);

			_eventServiceMock.Setup(m => m.Delete(defaultEventGuid))
				.ReturnsAsync(true)
				.Callback(() => IsEventDeleted = true);

			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>()))
				.ReturnsAsync(true);
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
			await _bookingService.ConfirmBookingAsync(firstBooking.Id);
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
			await _bookingService.RejectedBookingAsync(firstBooking.Id);
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
			await _eventServiceMock.Object.Delete(defaultEventGuid);

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
			_eventServiceMock.Setup(m => m.TryReserveSeats(It.IsAny<Guid>(), It.IsAny<int>())).ReturnsAsync(true);

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
				.ReturnsAsync(() =>
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
				.ReturnsAsync(() =>
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
				.ReturnsAsync(() => false);
			
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
			await _bookingService.ConfirmBookingAsync(firstBooking.Id);

			//Assert
			Assert.Equal(BookingStatus.Confirmed, firstBooking.Status);
			Assert.NotNull(firstBooking.ProcessedAt);
		}

		[Fact]
		public async Task Rejected_ValidBooking_ReturnsTrue()
		{
			//Act
			var firstBooking = await _bookingService.CreateBookingAsync(defaultEventGuid);
			await _bookingService.RejectedBookingAsync(firstBooking.Id);

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
				.ReturnsAsync(() =>
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
				.ReturnsAsync(() =>
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
						using var scope = _serviceProvider.CreateScope();
						var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
						var booking = await bookingService.CreateBookingAsync(defaultEventGuid);
						Interlocked.Increment(ref successfulBookings);
					}
					catch
					{
						Interlocked.Increment(ref exceptionsBookings);
					}
				});

			//Assert
			Assert.Equal(15, exceptionsBookings);
			Assert.Equal(5, successfulBookings);
		}

		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
			_serviceProvider.Dispose();
		}
	}
}
