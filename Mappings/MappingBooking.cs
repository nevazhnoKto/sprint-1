using AutoMapper;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.Mappings
{
	/// <summary>
	/// Маппинг Booking и BookingDto.
	/// </summary>
	public class MappingBooking : Profile
	{
		/// <summary>
		/// Маппинг Booking и BookingDto.
		/// </summary>
		public MappingBooking()
		{
			CreateMap<Booking, BookingDto>().ReverseMap();
		}
	}
}
