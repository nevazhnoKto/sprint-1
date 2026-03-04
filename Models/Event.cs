namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Модель Event.
	/// </summary>
	public class Event
	{
		/// <summary>
		/// ID события.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Заголовок события.
		/// </summary>
		public string? Title { get; set; }

		/// <summary>
		/// Описание события.
		/// </summary>
		public string? Description { get; set; }

		/// <summary>
		/// Время начала события.
		/// </summary>
		public DateTime StartAt { get; set; }

		/// <summary>
		/// Время окончания события.
		/// </summary>
		public DateTime EndAt { get; set; }
	}
}
