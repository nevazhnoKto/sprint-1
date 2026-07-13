using Event.Application.Models;
using Event.Domain.Models;

namespace Event.Application.Interfaces
{
	/// <summary>
	/// Интерфейс сервиса обработки событий.
	/// </summary>
	public interface IEventService
	{
		/// <summary>
		/// Получить все события, соответвующие параметрам запроса.
		/// </summary>
		/// <param name="title">Поиск по заголовку.</param>
		/// <param name="from">Поиск по дате начала события.</param>
		/// <param name="to">Поиск по дате окончания события.</param>
		/// <param name="page">Номер страницы.</param>
		/// <param name="pageSize">Количество событий на странице.</param>
		/// <returns></returns>
		Task<PaginatedResult> GetAll(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10);

		/// <summary>
		/// Получить событие по Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <returns>Информация по найденному событию.</returns>
		Task<EventDto?> GetById(Guid id);

		/// <summary>
		/// Создать новое событие.
		/// </summary>
		/// <param name="eventCustom">Данные нового события.</param>
		/// <returns>True - если событие удачно добавлено.</returns>
		Task<bool> Create(CreateEventRequestDto eventCustom);

		/// <summary>
		/// Обновить событие по его Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <param name="eventCustom">Данные, которыми обновить событие.</param>
		/// <returns>True - если обновление прошло удачно.</returns>
		Task<bool> Update(Guid id, UpdateEventRequestDto eventCustom);

		/// <summary>
		/// Удалить событие по его Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <returns>True - елси событие успешно удалено.</returns>
		Task<bool> Delete(Guid id);


		/// <summary>
		/// Попытка резервирования места на событие.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <param name="count">Количество для резервации.</param>
		/// <returns>Возвращает false, если свободных мест недостаточно.</returns>
		Task<bool> TryReserveSeats(Guid id, int count = 1);

		/// <summary>
		/// Отмена освобождения места на событие.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <param name="count">Количество мест для отмены.</param>
		/// <returns></returns>
		Task<bool> ReleaseSeats(Guid id, int count = 1);

		/// <summary>
		/// Возвращает 10 событий с наибольшим процентом проданных мест
		/// </summary>
		/// <returns>Список событий.</returns>
		Task<List<EventDto?>> GetTop10Events();
	}
}
