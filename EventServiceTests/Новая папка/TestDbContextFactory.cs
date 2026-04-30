using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiTamakulov.DataAccess;
using WebApiTamakulov.Services;

namespace EventServiceTests.Новая_папка
{
	static public class TestDbContextFactory
	{
		public static (AppDbContext context, EventService eventService) Create()
		{
			var dbName = Guid.NewGuid().ToString();
			var services = new ServiceCollection();

			services.AddDbContext<AppDbContext>(options =>
				options.UseInMemoryDatabase(dbName));

			var serviceProvider = services.BuildServiceProvider();
			var context = serviceProvider.GetRequiredService<AppDbContext>();

			var loggerMock = new Mock<ILogger<EventService>>();
			var repository = new EventRepository(context);
			var eventService = new EventService(loggerMock.Object, repository);

			// Возвращаем Tuple
			return (context, eventService);
		}
	}
}
