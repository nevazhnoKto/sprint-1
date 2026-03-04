using AutoMapper;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Mappings
{
	/// <summary>
	/// Маппинг Event и EventDto.
	/// </summary>
	public class MappingProfile : Profile
	{
		/// <summary>
		/// Маппинг Event и EventDto.
		/// </summary>
		public MappingProfile()
		{
			CreateMap<Event, EventDto>().ReverseMap();
		}
	}
}
