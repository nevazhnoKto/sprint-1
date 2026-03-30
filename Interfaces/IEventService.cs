using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
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
		PaginatedResult GetAll(string? title, DateTime? from, DateTime? to, int page = 1, int pageSize = 10);

		/// <summary>
		/// Получить событие по Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <returns>Информация по найденному событию.</returns>
		Event? GetById(int id);

		/// <summary>
		/// Создать новое событие.
		/// </summary>
		/// <param name="eventCustom">Данные нового события.</param>
		/// <returns>True - если событие удачно добавлено.</returns>
		bool Create(Event eventCustom);

		/// <summary>
		/// Обновить событие по его Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <param name="eventCustom">Данные, которыми обновить событие.</param>
		/// <returns>True - если обновление прошло удачно.</returns>
		bool Update(int id, Event eventCustom);

		/// <summary>
		/// Удалить событие по его Id.
		/// </summary>
		/// <param name="id">Id события.</param>
		/// <returns>True - елси событие успешно удалено.</returns>
		bool Delete(int id);

		/// <summary>
		/// Костыль для тестов, чтобы сбрасывать статик переменную.
		/// </summary>
		void Reset();

	}
}
