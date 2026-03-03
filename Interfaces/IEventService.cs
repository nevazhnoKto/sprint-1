using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
{
	public interface IEventService
	{
		IEnumerable<Event> GetAll();
		Event? GetById(int id);
		bool Create(Event eventCustom);
		bool Update(int id, Event eventCustom);
		bool Delete(int id);

	}
}
