using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using Testcontainers.PostgreSql;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Models;
using WebApiTamakulov.Repositories;

namespace IntegrationTests
{
	[Collection("Database")]
	public class EventRepositoryTests: IAsyncLifetime
	{
		private readonly DatabaseFixture _dbFixture;
		public EventRepositoryTests(DatabaseFixture dbFixture)
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
		public async Task Migrations_CreateBookingsTable()
		{
			// Arrange
			await using var context = await _dbFixture.CreateContext();

			// Act
			var tableExists = await TableExistsAsync(context, "Bookings");

			// Assert
			Assert.True(tableExists, "Bookings table should exist after migration");
		}

		[Fact]
		public async Task Migrations_CreateEventsTable()
		{
			// Arrange
			await using var context = await _dbFixture.CreateContext();

			// Act
			var tableExists = await TableExistsAsync(context, "Events");

			// Assert
			Assert.True(tableExists, "Events table should exist after migration");
		}

		[Fact]
		public async Task CreateEvent_SavesEventToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			var eventRepository = new EventRepository(context);

			//Act
			await eventRepository.AddEvent(newEvent);

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var checkEvent = await assertContext.Events.FirstAsync();
			Assert.Equal(newEvent.Title, checkEvent.Title);
		}
		/*
		[Fact]
		public async Task DeleteEvent_DeleteEventToDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			await eventRepository.DeleteEventById(new Guid("00000000-0000-0000-0000-000000000001"));

			//Assert
			await using var assertContext = await CreateContext();
			var checkEvent = await assertContext.Events.FirstOrDefaultAsync();
			Assert.Null(checkEvent);
		}

		[Fact]
		public async Task GetEventById_GetEventFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			var findedEvent = await eventRepository.GetEventById(new Guid("00000000-0000-0000-0000-000000000001"));

			//Assert
			Assert.Equal(newEvent.Title, findedEvent.Title);
		}

		[Fact]
		public async Task GetEvents_GetAllEventsFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			var newEvent2 = new Event(new Guid("00000000-0000-0000-0000-000000000002"), "Второе событие", "Тоже классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.AddRange(newEvent, newEvent2);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			var findedEvents = await eventRepository.GetEvents();

			//Assert
			Assert.Equal(2, findedEvents.Count);
		}

		[Fact]
		public async Task GetEventsFiltered_GetFiltretedEventsFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			var newEvent2 = new Event(new Guid("00000000-0000-0000-0000-000000000002"), "Второе событие", "Тоже классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.AddRange(newEvent, newEvent2);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			var findedEvents = await eventRepository.GetEventsFiltered("Первое событие", null, null);

			//Assert
			Assert.NotNull(findedEvents.FirstOrDefault());
			Assert.Equal("Очень классное событие", findedEvents.FirstOrDefault().Description);
		}

		[Fact]
		public async Task UpdateEvent_UpdateEventFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			newEvent.Title = "Обновленное классное событие";
			await eventRepository.UpdateAsync(newEvent);

			//Assert
			await using var asserctContext = await CreateContext();
			var assertEvent = asserctContext.Events.FirstOrDefault();
			Assert.Equal("Обновленное классное событие", assertEvent.Title);
		}
		[Fact]
		public async Task UpdateEventByIndex_UpdateEventByIndexFromDataBase()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			context.Events.Add(newEvent);
			await context.SaveChangesAsync();

			//Act
			var eventRepository = new EventRepository(await CreateContext());
			newEvent.Title = "Обновленное классное событие";
			await eventRepository.UpdateEventByIndex(new Guid("00000000-0000-0000-0000-000000000001"), newEvent);

			//Assert
			await using var asserctContext = await CreateContext();
			var assertEvent = asserctContext.Events.FirstOrDefault();
			Assert.Equal("Обновленное классное событие", assertEvent.Title);
		}

		[Fact]
		public async Task CreateEvent_DuplicateId_ThrowsDbUpdateException()
		{
			await ResetDatabaseAsync();

			//Arrange
			await using var context = await CreateContext();
			var newEvent = new Event(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.UtcNow, DateTime.UtcNow.AddHours(2), 10);
			var eventRepository = new EventRepository(context);
			await eventRepository.AddEvent(newEvent);

			//Act, Assert
			await Assert.ThrowsAsync<DbUpdateException>(() => eventRepository.AddEvent(newEvent));
		}*/

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