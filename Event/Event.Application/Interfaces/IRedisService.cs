using Event.Domain.Models;

namespace Event.Application.Interfaces
{
	/// <summary>
	/// Сервис для работы с кэшем.
	/// </summary>
	public interface IRedisService
	{
		/// <summary>
		/// Получить данные по eventId.
		/// </summary>
		/// <param name="eventId">ИД события.</param>
		/// <returns>Модель события.</returns>
		Task<EventModel?> GetCacheForIdAsync(Guid eventId);

		/// <summary>
		/// Записать данные для события.
		/// </summary>
		/// <param name="eventId">ИД события.</param>
		/// <param name="value">Модель события.</param>
		/// <returns>Boolean</returns>
		Task<bool> SetCacheAsync(Guid eventId, EventModel value);

		/// <summary>
		/// Удалить информуцию события из кеша.
		/// </summary>
		/// <param name="eventId">ИД события.</param>
		/// <returns>Boolean</returns>
		Task<bool> DeleteCacheAsync(Guid eventId);

		/// <summary>
		/// Возвращает 10 событий с наибольшим процентом проданных мест.
		/// </summary>
		/// <returns>Список событий.</returns>
		Task<List<EventModel>?> GetTop10EventsAsync();

		/// <summary>
		///  Заполняет в кэш 10 событий с наибольшим процентом проданных мест.
		/// </summary>
		/// <param name="topEvents">Список событий.</param>
		/// <returns>Удачное обновление кэша.</returns>
		Task<bool> SetTop10EventsAsync(List<EventModel> topEvents);
	}
}
