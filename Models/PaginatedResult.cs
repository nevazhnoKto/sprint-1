namespace WebApiTamakulov.Models
{
	public class PaginatedResult
	{
		/// <summary>
		/// Общее количество событий
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Массив самих событий
		/// </summary>
		public List<Event> Items { get; set; } = new();

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
