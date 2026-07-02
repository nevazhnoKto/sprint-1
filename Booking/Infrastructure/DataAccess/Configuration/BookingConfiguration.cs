using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Booking.Domain.Models;

namespace Booking.Infrastructure.DataAccess.Configuration
{
#pragma warning disable CS1591
	public class BookingConfiguration : IEntityTypeConfiguration<BookingModel>
	{
		public void Configure(EntityTypeBuilder<BookingModel> builder)
		{
			builder.ToTable("bookings");

			builder.HasKey(b => b.Id);

			builder.Property(b => b.Id)
				.HasColumnName("id")
				.ValueGeneratedNever();

			builder.Property(b => b.EventId)
				.HasColumnName("event_id")
				.IsRequired();

			builder.Property(b => b.Status)
				.HasColumnName("status")
				.IsRequired()
				.HasConversion<string>()
				.HasMaxLength(20);

			builder.Property(b => b.CreatedAt)
				.HasColumnName("created_at")
				.IsRequired();

			builder.Property(b => b.ProcessedAt)
				.HasColumnName("processed_at");
		}
	}
#pragma warning restore CS1591
}
