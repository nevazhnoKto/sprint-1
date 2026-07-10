
using Event.Application.Interfaces;
using Event.Domain.Models;
using Event.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Event.Infrastructure.Repositories
{
#pragma warning disable CS1591
	public class InboxRepository : IInboxRepository
	{
		private readonly AppDbContext _context;
		private readonly ILogger<InboxRepository> _logger;

		public InboxRepository(AppDbContext context, ILogger<InboxRepository> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task<bool> TrySaveAsync(Guid bookingId, string nameKafkaEvent)
		{
			try
			{
				// Создаем запись для журнала Inbox
				var inboxMessage = new InboxMessage
				{
					Id = bookingId,
					MessageName = nameKafkaEvent,
					ProcessedAt = DateTimeOffset.UtcNow
				};

				_context.InboxMessages.Add(inboxMessage);

				// Пытаемся сохранить в Postgres
				await _context.SaveChangesAsync();

				return true;
			}
			catch (DbUpdateException)
			{
				// Сюда мы попадаем, если в базе уже есть строка с таким же id_booking И message
				_logger.LogWarning("Inbox: Обнаружен дубликат события '{EventName}' для брони {BookingId}. Обработка заблокирована базой данных.",
					nameKafkaEvent, bookingId);
				
				_context.ChangeTracker.Clear();
				return false;
			}
		}
	}
#pragma warning restore CS1591
}
