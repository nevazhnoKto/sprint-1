using Application.Interfaces;
using Application.Models;
using Application.Services;
using Domain.Models;
using Infrastructure.DataAccess;
using Infrastructure.Repositories;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ServiceTests;

public class EventServiceTests: IDisposable
{
	private readonly IEventService _eventService;
	private readonly AppDbContext _context;
	private readonly ServiceProvider _serviceProvider;
	private readonly IMapper _mapper;

	public EventServiceTests()
	{

		var services = new ServiceCollection();

		// Регистрируем синглтон конфигурации и scoped маппер
		var config = new TypeAdapterConfig();
		services.AddSingleton(config);
		services.AddScoped<IMapper, ServiceMapper>();

		var dbName = Guid.NewGuid().ToString();
		services.AddDbContext<AppDbContext>(options =>
			options.UseInMemoryDatabase(dbName));

		services.AddScoped<IEventRepository, EventRepository>();

		var loggerMock = new Mock<ILogger<EventService>>();
		services.AddScoped(_ => loggerMock.Object);

		services.AddScoped<IEventService, EventService>();

		_serviceProvider = services.BuildServiceProvider();

		_context = _serviceProvider.GetRequiredService<AppDbContext>();
		_eventService = _serviceProvider.GetRequiredService<IEventService>();

		var newEvent = new CreateEventRequestDto(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10);
		
		_eventService.Create(newEvent);
	}

	[Fact]
	public async Task GetAllEvents_NoFilter_ReturnsEvents()
	{
		//Arrange
		var expected = 1;

		//Act
		var result = await _eventService.GetAll("", null, null);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage);
	}

	[Fact]
	public async Task GetEventById_ExistingId_ReturnsEvent()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var expectedTitle = "Первое событие";
		var expectedDescription = "Очень классное событие";

		//Act
		var result = await _eventService.GetById(idEvent);

		//Assert
		Assert.Equal(expectedTitle, result?.Title);
		Assert.Equal(expectedDescription, result?.Description);
	}

	[Fact]
	public async Task UpdateEvent_ExistingEvent_ReturnsTrue()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var newEvent = GetNewEventForUpdate();

		//Act
		var result = await _eventService.Update(idEvent, newEvent);

		//Assert
		Assert.True(result);
	}

	[Fact]
	public async Task RemoveAsync_ExistingEvent_ReturnsTrue()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");

		//Act
		var result = await _eventService.Delete(idEvent);

		//Assert
		Assert.True(result);
	}

	[Theory]
	[InlineData("Первое событие", true)]
	[InlineData("Первое", true)]
	public async Task GetEventsByTitle_ValidTitle_ReturnsEvents(string title, bool expected)
	{
		//Act
		var result = await _eventService.GetAll(title, null, null);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Theory]
	[MemberData(nameof(GetValidDates))]
	public async Task GetEventsByDateRange_ValidDates_ReturnsEvents(DateTime from, DateTime to, bool expected)
	{
		//Act
		var result = await _eventService.GetAll("", from, to);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}	

	[Theory]
	[InlineData(1, 10 , true)]
	[InlineData(1, 20, true)]
	public async Task GetAllEvents_ValidPageNumber_ReturnsEvents(int page, int pageSize, bool expected)
	{
		//Act
		var result = await _eventService.GetAll("", null, null, page, pageSize);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Theory]
	[MemberData(nameof(GetValidCombineData))]
	public async Task GetAllEvents_CombineValidData_ReturnsEvents(string title, DateTime from, DateTime to, int page, int pageSize, bool expected)
	{
		//Act
		var result = await _eventService.GetAll(title, from, to, page, pageSize);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Fact]
	public async Task GetEventById_NoExistingId_ReturnsNull()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");

		//Act
		var result = await _eventService.GetById(idEvent);

		//Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task UpdateEvent_NoExistingId_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");
		var newEvent = GetNewEventForUpdate();

		//Act
		var result = await _eventService.Update(idEvent, newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public async Task CreateEvent_NoValidDatas_ReturnsFalse()
	{
		//Arrange
		var newEvent = GetNewEvent();
		newEvent.StartAt = DateTime.Now.AddDays(1);
		newEvent.EndAt = DateTime.Now;

		//Act
		var result = await _eventService.Create(newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public async Task CreateEvent_RepeateId_ReturnsFalse()
	{
		//Arrange
		var newEvent = GetNewEvent();

		//Act
		await _eventService.Create(newEvent);
		var result = await _eventService.Create(newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public async Task UpdateEvent_NoValidDatas_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var newEvent = GetNewEventForUpdate();
		newEvent.StartAt = DateTime.Now.AddDays(1);
		newEvent.EndAt = DateTime.Now;

		//Act
		var result = await _eventService.Update(idEvent, newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public async Task RemoveAsync_NoExistingId_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");

		//Act
		var result = await _eventService.Delete(idEvent);

		//Assert
		Assert.False(result);
	}

	private CreateEventRequestDto GetNewEvent()
	{
		return new CreateEventRequestDto(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10);
	}

	private UpdateEventRequestDto GetNewEventForUpdate()
	{
		return new UpdateEventRequestDto(new Guid("00000000-0000-0000-0000-000000000001"), "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10, 10);
	}

	public static IEnumerable<object[]> GetValidDates()
	{
		yield return new object[] { DateTime.Now.AddMinutes(-1), DateTime.Now.AddHours(3), true };
		yield return new object[] { DateTime.Now.AddHours(-1), DateTime.Now.AddHours(4), true };
	}

	public static IEnumerable<object[]> GetValidCombineData()
	{
		yield return new object[] { "Первое событие", DateTime.Now.AddMinutes(-1), DateTime.Now.AddHours(3), 1, 10, true };
		yield return new object[] { "Первое событие", DateTime.Now.AddHours(-11), DateTime.Now.AddHours(33), 1, 20, true };
	}

	public void Dispose()
	{
		_context.Database.EnsureDeleted();
		_context.Dispose();
		_serviceProvider.Dispose();
	}
}

