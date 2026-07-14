using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Event.Infrastructure.Models
{
	/// <summary>
	/// Класс с настройками Redis.
	/// </summary>
	public sealed class RedisSettings
	{
		/// <summary>
		/// Строка подключения.
		/// </summary>
		public string ConnectionString { get; set; } = string.Empty;

		/// <summary>
		/// TTL кэша для события.
		/// </summary>
		public int EventCacheTtlMinutes { get; set; } = 5;

		/// <summary>
		/// TTL кэша для топа событий с наибольшим процентом проданных мест.
		/// </summary>
		public int TopEventsCacheTtlMinutes { get; set; } = 15;
	}
}
