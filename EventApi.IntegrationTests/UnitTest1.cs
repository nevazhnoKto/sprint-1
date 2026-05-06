using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using WebApiTamakulov.DataAccess;

namespace EventApi.IntegrationTests
{
	public class UnitTest1
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
			await using var context = CreateContext();
			await context.Database.ExecuteSqlRawAsync(
				"TRUNCATE TABLE books, authors RESTART IDENTITY CASCADE");
		}

		private AppDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<AppDbContext>()
				.UseNpgsql(_postgres.GetConnectionString())
				.Options;

			var context = new AppDbContext(options);
			context.Database.EnsureCreated();
			return context;
		}

		[Fact]
		public void Test1()
		{

		}
	}
}