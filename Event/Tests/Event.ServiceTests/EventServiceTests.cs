using Event.Application.Interfaces;
using Event.Application.Models;
using Event.Application.Services;
using Event.Domain.Models;
using Event.Infrastructure.DataAccess;
using Event.Infrastructure.Repositories;
using Event.Infrastructure.Service;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Event.ServiceTests
{
	public class EventServiceTests: IDisposable
	{
		private readonly IEventService _eventService;
		private readonly ServiceProvider _serviceProvider;
		private readonly Mock<IEventRepository> _eventRepository;
		private readonly Mock<IRedisService> _redisService;
		private Guid _defaultEventGuid = new Guid("00000000-0000-0000-0000-000000000001");


		public EventServiceTests()
		{
			var services = new ServiceCollection();

			// Регистрируем синглтон конфигурации и scoped маппер
			var config = new TypeAdapterConfig();
			services.AddSingleton(config);
			services.AddScoped<IMapper, ServiceMapper>();

			// Регистрируем сервисы.
			services.AddScoped<IEventService, EventService>();

			_eventRepository = new Mock<IEventRepository>();
			services.AddScoped(_ => _eventRepository.Object);

			_redisService = new Mock<IRedisService>();
			services.AddScoped(_ => _redisService.Object);

			var loggerMock = new Mock<ILogger<EventService>>();
			services.AddScoped(_ => loggerMock.Object);

			_serviceProvider = services.BuildServiceProvider();

			_eventService = _serviceProvider.GetRequiredService<IEventService>();
		}

		[Fact]
		public void GetEventById_ReturnFromCache_NotCallRepository()
		{
			//Arrange
			_redisService.Setup(x => x.GetCacheForIdAsync(_defaultEventGuid)).ReturnsAsync(GetNewEventModel());

			//Act
			_eventService.GetById(_defaultEventGuid);

			//Assert
			_eventRepository.Verify(repo => repo.GetEventById(_defaultEventGuid), Times.Never);
		}

		[Fact]
		public void GetEventById_ReturnFromRepo_CallSetCache()
		{
			//Arrange
			_eventRepository.Setup(x => x.GetEventById(_defaultEventGuid)).ReturnsAsync(GetNewEventModel());

			//Act
			_eventService.GetById(_defaultEventGuid);

			//Assert
			_eventRepository.Verify(repo => repo.GetEventById(_defaultEventGuid), Times.Once);
			_redisService.Verify(redis => redis.SetCacheAsync(_defaultEventGuid, It.IsAny<EventModel>()), Times.Once);
		}

		[Fact]
		public void Update_UpdateRepoAndCache_CallSetCache()
		{
			//Arrange
			_eventRepository.Setup(x => x.UpdateEventByIndex(_defaultEventGuid, It.IsAny<EventModel>())).ReturnsAsync(true);

			//Act
			_eventService.Update(_defaultEventGuid, GetNewEventForUpdate());

			//Assert
			_eventRepository.Verify(repo => repo.UpdateEventByIndex(_defaultEventGuid, It.IsAny<EventModel>()), Times.Once);
			_redisService.Verify(redis => redis.SetCacheAsync(_defaultEventGuid, It.IsAny<EventModel>()), Times.Once);
		}

		[Fact]
		public void TryReserveSeats_ReserveSeats_CallSetCache()
		{
			//Arrange
			_eventRepository.Setup(x => x.GetEventById(_defaultEventGuid)).ReturnsAsync(GetNewEventModel());

			//Act
			_eventService.TryReserveSeats(_defaultEventGuid);

			//Assert
			_eventRepository.Verify(repo => repo.UpdateAsync(It.IsAny<EventModel>()), Times.Once);
			_redisService.Verify(redis => redis.SetCacheAsync(_defaultEventGuid, It.IsAny<EventModel>()), Times.Once);
		}

		[Fact]
		public void Delete_DeleteEvent_CallSetCache()
		{
			//Arrange
			_eventRepository.Setup(x => x.GetEventById(_defaultEventGuid)).ReturnsAsync(GetNewEventModel());

			//Act
			_eventService.Delete(_defaultEventGuid);

			//Assert
			_eventRepository.Verify(repo => repo.GetEventById(_defaultEventGuid), Times.Once);
			_redisService.Verify(redis => redis.DeleteCacheAsync(_defaultEventGuid), Times.Once);
		}

		public void Dispose()
		{
			_serviceProvider.Dispose();
		}

		private EventModel GetNewEventModel()
		{
			return new EventModel(_defaultEventGuid, "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10);
		}


		private UpdateEventRequestDto GetNewEventForUpdate()
		{
			return new UpdateEventRequestDto(_defaultEventGuid, "Первое событие", "Очень классное событие", DateTime.Now, DateTime.Now.AddHours(2), 10, 10);
		}
	}
}