using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiTamakulov.Interfaces;
using WebApiTamakulov.Models;
using WebApiTamakulov.Services;

namespace EventServiceTests;

public class EventServiceTests
{
	private readonly IEventService eventService;
	public EventServiceTests()
	{
		var loggerMock = new Mock<ILogger<EventService>>();
		eventService = new EventService(loggerMock.Object);
		eventService.Reset();
	}

	[Fact]
	public void CreateEvent_ValidEvent_ReturnsTrue()
	{
		//Arrange
		var newEvent = GetNewEvent();

		//Act
		var result = eventService.Create(newEvent);

		//Assert
		Assert.True(result);
	}

	[Fact]
	public void GetAllEvents_NoFilter_ReturnsEvents()
	{
		//Arrange
		var expected = 1;

		//Act
		var result = eventService.GetAll("", null, null);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage);
	}

	[Fact]
	public void GetEventById_ExistingId_ReturnsEvent()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var expectedTitle = "Первое событие";
		var expectedDescription = "Очень классное событие";

		//Act
		var result = eventService.GetById(idEvent);

		//Assert
		Assert.Equal(expectedTitle, result?.Title);
		Assert.Equal(expectedDescription, result?.Description);
	}

	[Fact]
	public void UpdateEvent_ExistingEvent_ReturnsTrue()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var newEvent = GetNewEvent();

		//Act
		var result = eventService.Update(idEvent, newEvent);

		//Assert
		Assert.True(result);
	}

	[Fact]
	public void RemoveAsync_ExistingEvent_ReturnsTrue()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");

		//Act
		var result = eventService.Delete(idEvent);

		//Assert
		Assert.True(result);
	}

	[Theory]
	[InlineData("Первое событие", true)]
	[InlineData("Первое", true)]
	public void GetEventsByTitle_ValidTitle_ReturnsEvents(string title, bool expected)
	{
		//Act
		var result = eventService.GetAll(title, null, null);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Theory]
	[MemberData(nameof(GetValidDates))]
	public void GetEventsByDateRange_ValidDates_ReturnsEvents(DateTime from, DateTime to, bool expected)
	{
		//Act
		var result = eventService.GetAll("", from, to);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}	

	[Theory]
	[InlineData(1, 10 , true)]
	[InlineData(1, 20, true)]
	public void GetAllEvents_ValidPageNumber_ReturnsEvents(int page, int pageSize, bool expected)
	{
		//Act
		var result = eventService.GetAll("", null, null, page, pageSize);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Theory]
	[MemberData(nameof(GetValidCombineData))]
	public void GetAllEvents_CombineValidData_ReturnsEvents(string title, DateTime from, DateTime to, int page, int pageSize, bool expected)
	{
		//Act
		var result = eventService.GetAll(title, from, to, page, pageSize);

		//Assert
		Assert.Equal(expected, result.CountCurrentPage == 1);
	}

	[Fact]
	public void GetEventById_NoExistingId_ReturnsNull()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");

		//Act
		var result = eventService.GetById(idEvent);

		//Assert
		Assert.Null(result);
	}

	[Fact]
	public void UpdateEvent_NoExistingId_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");
		var newEvent = GetNewEvent();

		//Act
		var result = eventService.Update(idEvent, newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public void CreateEvent_NoValidDatas_ReturnsFalse()
	{
		//Arrange
		var newEvent = GetNewEvent();
		newEvent.StartAt = DateTime.Now.AddDays(1);
		newEvent.EndAt = DateTime.Now;

		//Act
		var result = eventService.Create(newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public void CreateEvent_RepeateId_ReturnsFalse()
	{
		//Arrange
		var newEvent = GetNewEvent();
		newEvent.Id = new Guid("00000000-0000-0000-0000-000000000001");

		//Act
		var result = eventService.Create(newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public void UpdateEvent_NoValidDatas_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000001");
		var newEvent = GetNewEvent();
		newEvent.StartAt = DateTime.Now.AddDays(1);
		newEvent.EndAt = DateTime.Now;

		//Act
		var result = eventService.Update(idEvent, newEvent);

		//Assert
		Assert.False(result);
	}

	[Fact]
	public void RemoveAsync_NoExistingId_ReturnsFalse()
	{
		//Arrange
		var idEvent = new Guid("00000000-0000-0000-0000-000000000322");

		//Act
		var result = eventService.Delete(idEvent);

		//Assert
		Assert.False(result);
	}

	private Event GetNewEvent()
	{
		return new Event
		{
			Id = new Guid("00000000-0000-0000-0000-000000000002"),
			Title = "Новое событие",
			Description = "Потрясающее событие",
			StartAt = DateTime.Now,
			EndAt = DateTime.Now.AddHours(2)
		};
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
}
