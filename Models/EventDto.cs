using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using WebApiTamakulov.Interfaces;

namespace WebApiTamakulov.Models
{
	public class EventDto
	{
		/// <summary>
		/// ID события.
		/// </summary>
		[Required(ErrorMessage = "Значение ID обязательно для заполнения")]
		public int Id { get; set; }

		/// <summary>
		/// Заголовок события.
		/// </summary>
		[Required(ErrorMessage = "Значение Title обязательно для заполнения")]
		public string Title { get; set; }

		/// <summary>
		/// Заголовок события.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Время начала события.
		/// </summary>
		[Required(ErrorMessage = "Значение StartAt обязательно для заполнения")]
		public DateTime StartAt { get; set; }

		/// <summary>
		/// Время окончания события.
		/// </summary>
		[Required(ErrorMessage = "Значение EndAt обязательно для заполнения")]
		public DateTime EndAt { get; set; }
	}
}
