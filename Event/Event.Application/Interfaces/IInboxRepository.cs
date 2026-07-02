using Event.Domain.Models;

namespace Event.Application.Interfaces
{
	/// <summary>
	/// Репозиторий для управления событиями.
	/// </summary>
	public interface IInboxRepository
	{
		/// <summary>
		/// Пытается сохранить запись в БД с уникальным.
		/// </summary>
		Task<bool> TrySaveAsync(Guid bookingId, string nameKafkaEvent);
	}
}
