using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;
using WebApiTamakulov.DataAccess;

namespace IntegrationTests
{
	public class DatabaseFixture : IAsyncLifetime
	{
		private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();

		public async Task InitializeAsync()
		{
			await _postgres.StartAsync();
		}

		public async Task DisposeAsync()
		{
			await _postgres.DisposeAsync();
		}

		public async Task ResetDatabaseAsync()
		{
			await using var context = await CreateContext();
			await context.Database.ExecuteSqlRawAsync(
				"TRUNCATE TABLE events, bookings RESTART IDENTITY CASCADE");
		}

		public async Task<AppDbContext> CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseNpgsql(_postgres.GetConnectionString())
				.Options;

			var context = new AppDbContext(options);
			await context.Database.MigrateAsync();
			return context;
		}
	}
}
