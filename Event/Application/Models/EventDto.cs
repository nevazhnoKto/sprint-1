namespace Event.Application.Models
{
	/// <summary>
	/// Модель EventDto.
	/// </summary>
	public class EventDto
	{
		/// <summary>
		/// Конструктор Event.
		/// </summary>
		public EventDto(Guid id, string? title, string? description, DateTime startAt, DateTime endAt, int totalSeats)
		{
			Id = id;
			Title = title;
			Description = description;
			StartAt = startAt;
			EndAt = endAt;
			TotalSeats = totalSeats;
			AvailableSeats = totalSeats;
		}
		/// <summary>
		/// ID события.
		/// </summary>
		public Guid Id { get; set; }

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

		/// <summary>
		/// Общее количество мест на событии.
		/// </summary>
		public int? TotalSeats { get; set; }

		/// <summary>
		/// Текущее количество свободных мест.
		/// </summary>
		public int AvailableSeats { get; set; }
	}
}
