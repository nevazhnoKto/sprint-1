using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
{
	/// <summary>
	/// Репозиторий для управления событиями.
	/// </summary>
	public interface IEventRepository
	{
		/// <summary>
		/// Возвращает список всех событий.
		/// </summary>
		List<Event> GetEvents();

		/// <summary>
		/// Возвращает событие по его уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор события (Guid).</param>
		Event? GetEventById(Guid id);

		/// <summary>
		/// Добавляет новое событие в репозиторий.
		/// </summary>
		/// <param name="newEvent">Объект события для добавления.</param>
		void AddEvent(Event newEvent);

		/// <summary>
		/// Обновляет существующее событие.
		/// </summary>
		/// <param name="index">Целочисленный индекс события.</param>
		/// <param name="eventCustom">Объект события с новыми данными.</param>
		void UpdateEventByIndex(int index, Event eventCustom);
		
		/// <summary>
		/// Удаляет событие по уникальному идентификатору.
		/// </summary>
		/// <param name="id">Идентификатор события (Guid).</param>
		void DeleteEventById(Guid id);

		/// <summary>
		/// Костыль для тестов, чтобы сбрасывать статик переменную.
		/// </summary>
		void Reset();
	}
}
