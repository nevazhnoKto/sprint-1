using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
{
	/// <summary>
	/// Интерфейс сервиса обработки событий.
	/// </summary>
	public interface IEventService
	{
		List<Event> GetAll();
		Event? GetById(int id);
		bool Create(Event eventCustom);
		bool Update(int id, Event eventCustom);
		bool Delete(int id);

	}
}
