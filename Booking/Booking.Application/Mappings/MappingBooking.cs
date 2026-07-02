using Booking.Application.Models;
using Booking.Domain.Models;
using Mapster;

namespace Booking.Application.Mappings
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
			config.NewConfig<BookingModel, BookingDto>()
				.TwoWays();
		}
	}
}
