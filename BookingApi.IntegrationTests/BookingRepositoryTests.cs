using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using Testcontainers.PostgreSql;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Enums;
using WebApiTamakulov.Models;
using WebApiTamakulov.Repositories;

namespace BookingApi.IntegrationTests
{
	public class BookingRepositoryTests: IAsyncLifetime
	{
		private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
		.WithImage("postgres:16-alpine")
		.Build();

		public async Task InitializeAsync()
		{
			await _postgres.StartAsync();
		}

		public async Task DisposeAsync()
		{
			await _postgres.DisposeAsync();
		}

		private async Task ResetDatabaseAsync()
		{
			await using var context = await CreateContext();
			await context.Database.ExecuteSqlRawAsync(
				"TRUNCATE TABLE events, bookings RESTART IDENTITY CASCADE");
		}

		private async Task<AppDbContext> CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseNpgsql(_postgres.GetConnectionString())
				.Options;

			var context = new AppDbContext(options);
			await context.Database.MigrateAsync();
			return context;
		}

		[Fact]
		public async Task Migrations_CreateBookingsTable()
		{
			await ResetDatabaseAsync();

			// Arrange
			await using var context = await CreateContext();

			// Act
			var tableExists = await TableExistsAsync(context, "Bookings");

			// Assert
			Assert.True(tableExists, "Bookings table should exist after migration");
		}

		[Fact]
		public async Task Migrations_CreateEventsTable()
		{
			await ResetDatabaseAsync();

			// Arrange
			await using var context = await CreateContext();

			// Act
			var tableExists = await TableExistsAsync(context, "Events");

			// Assert
			Assert.True(tableExists, "Events table should exist after migration");
		}

		
		[Fact]
		public async Task CreateBooking_SavesBookingToDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			var booking = await bookingRepository.AddBooking(new Guid("00000000-0000-0000-0000-000000000001"));

			//Assert
			await using var assertContext = await CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Equal(newBooking.EventId, booking.EventId);
		}

		[Fact]
		public async Task DeleteBooking_DeleteBookingToDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			await bookingRepository.DeleteBookingById(booking.Id);

			//Assert
			await using var assertContext = await CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Null(newBooking);
		}

		[Fact]
		public async Task GetBookingById_GetBookingByIdFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			var bookingDb = await bookingRepository.GetBookingById(booking.Id);

			//Assert
			Assert.Equal(booking.Id, bookingDb.Id);
			Assert.Equal(booking.EventId, bookingDb.EventId);
		}

		[Fact]
		public async Task GetBookingByStatus_GetBookingStatusFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
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
			Assert.Equal(booking2.Id, bookingDb.FirstOrDefault().Id);
			Assert.Equal(booking2.EventId, bookingDb.FirstOrDefault().EventId);
		}

		[Fact]
		public async Task UpdateBooking_UpdateBookingToDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			var booking = new Booking(new Guid("00000000-0000-0000-0000-000000000001"));
			context.Bookings.Add(booking);
			await context.SaveChangesAsync();

			//Act
			var bookingRepository = new BookingRepository(context);
			await bookingRepository.UpdateBooking(booking.Id, BookingStatus.Confirmed);

			//Assert
			await using var assertContext = await CreateContext();
			var newBooking = assertContext.Bookings.FirstOrDefault();
			Assert.Equal(BookingStatus.Confirmed, newBooking.Status);
		}

		private async Task<bool> TableExistsAsync(AppDbContext context, string tableName)
		{
			var connection = context.Database.GetDbConnection();
			if (connection.State != ConnectionState.Open)
				await connection.OpenAsync();

			var sql = @"
            SELECT COUNT(*)
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_name = @tableName";

			await using var command = connection.CreateCommand();
			command.CommandText = sql;

			var parameter = command.CreateParameter();
			parameter.ParameterName = "@tableName";
			parameter.Value = tableName.ToLower();
			command.Parameters.Add(parameter);

			var result = await command.ExecuteScalarAsync();
			return Convert.ToInt64(result) > 0;
		}
	}
}