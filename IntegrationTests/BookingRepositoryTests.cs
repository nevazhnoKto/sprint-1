using IntegrationTests.Fixture;
using WebApiTamakulov.Enums;
using WebApiTamakulov.Models;
using WebApiTamakulov.Repositories;

namespace IntegrationTests
{
	[Collection("Database")]
	public class BookingRepositoryTests: IAsyncLifetime
	{
		private readonly DatabaseFixture _dbFixture;
		public BookingRepositoryTests(DatabaseFixture dbFixture)
		{
			_dbFixture = dbFixture;
		}

		public async Task InitializeAsync()
		{
			// Перед началом каждого теста очистить БД.
			await _dbFixture.ResetDatabaseAsync();
		}

		public async Task DisposeAsync()
		{
		}

		[Fact]
		public async Task CreateBooking_SavesBookingToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			var booking = await bookingRepository.AddBooking(new Guid("00000000-0000-0000-0000-000000000001"));

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Equal(newBooking!.EventId, booking.EventId);
		}

		[Fact]
		public async Task DeleteBooking_DeleteBookingToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			await bookingRepository.DeleteBookingById(booking.Id);

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Null(newBooking);
		}

		[Fact]
		public async Task GetBookingById_GetBookingByIdFromDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			var bookingDb = await bookingRepository.GetBookingById(booking.Id);

			//Assert
			Assert.Equal(booking.Id, bookingDb!.Id);
			Assert.Equal(booking.EventId, bookingDb!.EventId);
		}

		[Fact]
		public async Task GetBookingByStatus_GetBookingStatusFromDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			booking.Status = BookingStatus.Rejected;
			var booking2 = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.AddRange(booking, booking2);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			var bookingDb = await bookingRepository.GetBookingsByStatus(BookingStatus.Pending);

			//Assert
			Assert.Equal(booking2.Id, bookingDb.First().Id);
			Assert.Equal(booking2.EventId, bookingDb.First().EventId);
		}

		[Fact]
		public async Task UpdateBooking_UpdateBookingToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			await bookingRepository.UpdateBooking(booking.Id, BookingStatus.Confirmed);

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Equal(BookingStatus.Confirmed, newBooking!.Status);
		}
	}
}