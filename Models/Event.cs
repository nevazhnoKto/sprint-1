namespace WebApiTamakulov.Models
{
	/// <summary>
	/// Модель Event.
	/// </summary>
	public class Event
	{
		private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

		/// <summary>
		/// Пустой конструктор для EF Core.
		/// </summary>
		private Event()
		{

		}

		/// <summary>
		/// Конструктор Event.
		/// </summary>
		public Event(Guid id, string? title, string? description, DateTime startAt, DateTime endAt, int totalSeats)
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
		public Guid Id { get; private set; }

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
		public int AvailableSeats { get; private set; }

		/// <summary>
		/// Попытка резервирования места на событие.
		/// </summary>
		/// <param name="count">Количество для резервации.</param>
		public async Task<bool> TryReserveSeats(int count = 1)
		{
			await _processingSemaphore.WaitAsync();
			try
			{
				if (AvailableSeats >= count)
				{
					AvailableSeats -= count;
					return true;
				}
				return false;
			}
			finally
			{
				_processingSemaphore.Release();
			}
		}

		/// <summary>
		/// Отмена освобождения места на событие.
		/// </summary>
		/// <param name="count">Количество мест для отмены.</param>
		public async Task<bool> ReleaseSeats(int count = 1)
		{
			await _processingSemaphore.WaitAsync();
			try
			{
				if (AvailableSeats + count <= TotalSeats)
				{
					AvailableSeats += count;
					return true;
				}
				return false;
			}
			finally
			{
				_processingSemaphore.Release();
			}
		}

		/// <summary>
		/// Коллекция Bookings.
		/// </summary>
		public List<Booking> Bookings { get; set; }
	}
}
