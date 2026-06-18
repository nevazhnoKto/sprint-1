using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;
using IntegrationTests.Fixture;

namespace IntegrationTests
{
	[Collection("Database")]
	public class UserRepositoryTests : IAsyncLifetime
	{
		private readonly DatabaseFixture _dbFixture;
		public UserRepositoryTests(DatabaseFixture dbFixture)
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
		public async Task CreateUser_SavesUserToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			
			//Act
			var user = new User("login", "hashPassword", Roles.User);
			var userRepository = new UserRepository(context);
			await userRepository.AddUser(user);

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var newBooking = assertContext.Users.FirstOrDefault();
			Assert.Equal(newBooking!.Id, user.Id);
		}

		[Fact]
		public async Task DeleteUser_DeleteUserToDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var user = new User("login", "hashPassword", Roles.User);
			context.Users.Add(user);
			await context.SaveChangesAsync();
			

			//Act
			var userRepository = new UserRepository(context);
			await userRepository.DeleteUserById(user.Id);

			//Assert
			await using var assertContext = await _dbFixture.CreateContext();
			var newUser = assertContext.Users.FirstOrDefault(u => u.Id == user.Id);
			Assert.Null(newUser);
		}

		[Fact]
		public async Task GetUserById_GetUserByIdFromDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var user = new User("login", "hashPassword", Roles.User);
			context.Users.Add(user);
			await context.SaveChangesAsync();

			//Act
			var userRepository = new UserRepository(context);
			var userDb = await userRepository.GetUserByLogin(user.Login);

			//Assert
			Assert.Equal(user.Id, userDb!.Id);
			Assert.Equal(user.Login, userDb!.Login);
		}

		[Fact]
		public async Task GetUsers_GetUsersFromDataBase()
		{
			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var user = new User("login", "hashPassword", Roles.User);
			var user2 = new User("login2", "hashPassword", Roles.Admin);
			context.Users.AddRange(user, user2);
			await context.SaveChangesAsync();

			//Act
			var userRepository = new UserRepository(context);
			var usersDb = await userRepository.GetUsers();

			//Assert
			Assert.Equal(2, usersDb.Count());
		}

		[Fact]
		public async Task UpdateUserByIndex_GetBookingUpdatedStatusFromDataBase()
		{

			//Arrange
			await using var context = await _dbFixture.CreateContext();
			var user = new User("login", "hashPassword", Roles.User);			
			context.Users.Add(user);
			await context.SaveChangesAsync();

			//Act
			user.Role = Roles.Admin;
			var userRepository = new UserRepository(context);
			await userRepository.UpdateUserByIndex(user.Id, user);
			var newUser = await userRepository.GetUserByLogin(user.Login);

			//Assert
			Assert.Equal(Roles.Admin, newUser!.Role);
		}
	}
}