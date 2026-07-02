using Application.Models;
using Domain.Models;
using Mapster;

namespace Application.Mappings
{
	/// <summary>
	/// Маппинг Booking и BookingDto.
	/// </summary>
	public class MappingBooking : IRegister
	{
		/// <summary>
		/// Регистрация маппингов.
		/// </summary>
		public void Register(TypeAdapterConfig config)
		{
			// Базовый маппинг Event <-> EventResponseDto
			config.NewConfig<Booking, BookingDto>()
				.TwoWays();
		}
	}
}
