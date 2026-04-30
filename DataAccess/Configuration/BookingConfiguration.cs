using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.DataAccess.Configuration
{
	public class BookingConfiguration : IEntityTypeConfiguration<Booking>
	{
		public void Configure(EntityTypeBuilder<Booking> builder)
		{
			builder.ToTable("Booking");

			builder.HasKey(b => b.Id);

			builder.Property(b => b.Id)
				.ValueGeneratedNever(); // Идентификатор генерируется в коде

			builder.Property(b => b.Id).IsRequired();
			builder.Property(b => b.EventId).IsRequired();
			builder.Property(b => b.Status).HasConversion<string>() // Конвертация enum в строку
				.IsRequired()
				.HasMaxLength(50); // Максимальная длина для строки статуса;
			builder.Property(b => b.CreatedAt).IsRequired();
			builder.Property(b => b.ProcessedAt);

			builder.HasOne(b => b.EventMain)
				.WithMany(e => e.Bookings)
				.HasForeignKey(b => b.EventId)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}
