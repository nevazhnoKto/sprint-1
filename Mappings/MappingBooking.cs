using Mapster;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Mappings
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
