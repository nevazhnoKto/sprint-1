using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace Application.Models
{
	/// <summary>
	/// Query параметры для получения событий.
	/// </summary>
	public class GetEventsRequestDto
	{
		/// <summary>
		/// Поиск по названию события.
		/// </summary>
		[FromQuery(Name = "title")]
		[Description("Поиск по названию события")]
		public string? Title { get; set; }

		/// <summary>
		/// Фильтр по дате начала: события, которые начинаются не раньше указанной даты
		/// </summary>
		[FromQuery(Name = "from")]
		[Description("Фильтр по дате начала")]
		public DateTime? From { get; set; }

		/// <summary>
		/// Фильтр по дате окончания: события, которые заканчиваются не позже указанной даты
		/// </summary>
		[FromQuery(Name = "to")]
		[Description("Фильтр по дате окончания")]
		public DateTime? To { get; set; }

		/// <summary>
		/// Номер страницы (начиная с 1)
		/// </summary>
		[FromQuery(Name = "page")]
		[Description("Номер страницы")]
		[DefaultValue(1)]
		public int Page { get; set; } = 1;

		/// <summary>
		/// Количество элементов на странице
		/// </summary>
		[FromQuery(Name = "pageSize")]
		[Description("Количество элементов на странице")]
		[DefaultValue(10)]
		public int PageSize { get; set; } = 10;
	}
}
