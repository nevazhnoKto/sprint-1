using Event.Domain.Models;

namespace Event.Application.Interfaces
{
	/// <summary>
	/// Репозиторий для управления событиями.
	/// </summary>
	public interface IEventRepository
	{
		/// <summary>
		/// Возвращает список всех событий.
		/// </summary>
		Task<List<EventModel>> GetEvents();

		/// <summary>
		/// Возвращает отфильтрованные события.
		/// </summary>
		/// <param name="title">Заголовок.</param>
		/// <param name="from">Дата начала.</param>
		/// <param name="to">Дата конца.</param>
		Task<List<EventModel>> GetEventsFiltered(string? title, DateTime? from, DateTime? to);

		/// <summary>
		/// Возвращает событие по его уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор события (Guid).</param>
		Task<EventModel?> GetEventById(Guid id);

		/// <summary>
		/// Добавляет новое событие в репозиторий.
		/// </summary>
		/// <param name="newEvent">Объект события для добавления.</param>
		Task AddEvent(EventModel newEvent);

		/// <summary>
		/// Обновляет существующее событие.
		/// </summary>
		/// <param name="index">Целочисленный индекс события.</param>
		/// <param name="eventCustom">Объект события с новыми данными.</param>
		Task<bool> UpdateEventByIndex(Guid index, EventModel eventCustom);

		/// <summary>
		/// Удаляет событие по уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор события (Guid).</param>
		Task DeleteEventById(Guid id);

		/// <summary>
		/// Обновляет передаваемое событие.
		/// </summary>
		/// <param name="eventCustom">Обновленное событие.</param>
		/// <returns></returns>
		Task UpdateAsync(EventModel eventCustom);
	}
}
