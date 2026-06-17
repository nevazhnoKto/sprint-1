using Application.Models;
using Domain.Models;
using Mapster;

namespace Application.Mappings
{
	/// <summary>
	/// Маппинг Event
	/// </summary>
	public class MappingEvent : IRegister
	{
		/// <summary>
		/// Регистрация маппингов.
		/// </summary>
		public void Register(TypeAdapterConfig config)
		{
			// Базовый маппинг Event <-> EventResponseDto
			config.NewConfig<Event, EventDto>()
				.TwoWays(); // ReverseMap аналог

			// Маппинг для создания события (CreateEventRequestDto -> Event)
			config.NewConfig<CreateEventRequestDto, Event>()
				 .MapWith(src => new Event(Guid.NewGuid(),
											src.Title,
											src.Description,
											src.StartAt,
											src.EndAt,
											src.TotalSeats
				));

			// Маппинг для обновления события (UpdateEventRequestDto -> Event)
			config.NewConfig<UpdateEventRequestDto, Event>();
		}
	}
}
