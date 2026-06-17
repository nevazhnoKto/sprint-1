namespace Application.Models
{
	/// <summary>
	/// Модель запроса на обновление Event.
	/// </summary>
	public class UpdateEventRequestDto
	{
		public UpdateEventRequestDto(Guid id, string? title, string? description, DateTime startAt, DateTime endAt, int totalSeats, int availableSeats)
		{
			Id = id;
			Title = title;
			Description = description;
			StartAt = startAt;
			EndAt = endAt;
			TotalSeats = totalSeats;
			AvailableSeats = availableSeats;
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
		public int TotalSeats { get; set; }

		/// <summary>
		/// Текущее количество свободных мест.
		/// </summary>
		public int AvailableSeats { get; set; }
	}
}
