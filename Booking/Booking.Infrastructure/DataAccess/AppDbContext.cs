using Booking.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.DataAccess
{
	/// <summary>
	/// Контекст базы данных для работы с событиями и бронированиями.
	/// Предоставляет доступ к коллекциям Event и Booking, а также настраивает конфигурацию моделей.
	/// </summary>
	public class AppDbContext : DbContext
	{
		/// <summary>
		/// Инициализирует новый экземпляр контекста базы данных.
		/// </summary>
		/// <param name="options">Настройки подключения к базе данных и другие параметры контекста.</param>
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		/// <summary>
		/// Получает набор сущностей Booking (бронирований).
		/// Используется для выполнения CRUD операций над бронированиями.
		/// </summary>
		public DbSet<BookingModel> Bookings => Set<BookingModel>();

		/// <summary>
		/// Настраивает модели и их связи при создании модели базы данных.
		/// </summary>
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}
