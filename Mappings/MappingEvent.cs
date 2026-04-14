using AutoMapper;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Mappings
{
	/// <summary>
	/// Маппинг Event и EventDto.
	/// </summary>
	public class MappingEvent : Profile
	{
		/// <summary>
		/// Маппинг Event и EventDto.
		/// </summary>
		public MappingEvent()
		{
			// Базовый маппинг Event <-> EventDto
			CreateMap<Event, EventResponseDto>().ReverseMap();

			// Маппинг для создания события (CreateEventRequestDto -> Event)
			CreateMap<CreateEventRequestDto, Event>()
				.ForMember(dest => dest.AvailableSeats, opt => opt.MapFrom(src => src.TotalSeats));

			// Маппинг для обновления события (UpdateEventRequestDto -> Event)
			CreateMap<UpdateEventRequestDto, Event>();
		}
	}
}
