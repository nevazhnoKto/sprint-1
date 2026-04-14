namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Модель Event.
	/// </summary>
	public class Event
	{
		private object _lock = new object();
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

		/// <summary>
		/// Попытка резервирования места на событие.
		/// </summary>
		/// <param name="count">Количество для резервации.</param>
		/// <returns>Возвращает false, если свободных мест недостаточно.</returns>
		public bool TryReserveSeats(int count = 1)
		{
			lock (_lock)
			{
				if (AvailableSeats >= count)
				{
					AvailableSeats -= count;
					return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Отмена резервирования места на событие.
		/// </summary>
		/// <param name="count">Количество мест для отмены.</param>
		/// <returns></returns>
		public bool ReleaseSeats(int count = 1)
		{
			return false;
		}
	}
}
