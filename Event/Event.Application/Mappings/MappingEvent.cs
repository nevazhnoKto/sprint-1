using Event.Application.Models;
using Event.Domain.Models;
using Mapster;

namespace Event.Application.Mappings
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
			config.NewConfig<EventModel, EventDto>()
				.TwoWays(); // ReverseMap аналог

			// Маппинг для создания события (CreateEventRequestDto -> Event)
			config.NewConfig<CreateEventRequestDto, EventModel>()
				 .MapWith(src => new EventModel(Guid.NewGuid(),
											src.Title,
											src.Description,
											src.StartAt,
											src.EndAt,
											src.TotalSeats
				));

			// Маппинг для обновления события (UpdateEventRequestDto -> Event)
			config.NewConfig<UpdateEventRequestDto, EventModel>();
		}
	}
}
