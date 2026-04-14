namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Результата пагинации для ответа в контроллере
	/// </summary>
	public class PaginatedResultDto
	{
		/// <summary>
		/// Общее количество событий
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Массив самих событий EventDto
		/// </summary>
		public List<EventResponseDto> Items { get; set; } = new();

		/// <summary>
		/// Текущая страница
		/// </summary>
		public int CurrentPage { get; set; }

		/// <summary>
		/// Количество элементов на текущей странице
		/// </summary>
		public int CountCurrentPage { get; set; }

		
	}
}
