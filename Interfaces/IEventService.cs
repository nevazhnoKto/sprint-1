using WebApiTamakulov.Models;

namespace WebApiTamakulov.Interfaces
{
	public interface IEventService
	{
		List<Event> GetAll();
		Event? GetById(int id);
		void Create(Event eventCustom);
		void Update(int id, Event eventCustom);
		void Delete(int id);

	}
}
