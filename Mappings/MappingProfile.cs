using AutoMapper;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Mappings
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			CreateMap<Event, EventDto>().ReverseMap();
		}
	}
}
