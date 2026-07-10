using Event.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.DataAccess.Configuration
{
#pragma warning disable CS1591
	public class EventConfiguration : IEntityTypeConfiguration<EventModel>
	{
		public void Configure(EntityTypeBuilder<EventModel> builder)
		{
			builder.ToTable("events");

			builder.HasKey(e => e.Id);

			builder.Property(e => e.Id)
				.HasColumnName("id")
				.ValueGeneratedNever();

			builder.Property(e => e.Title)
				.HasColumnName("title")
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(e => e.Description)
				.HasColumnName("description")
				.HasMaxLength(2000);

			builder.Property(e => e.StartAt)
				.HasColumnName("start_at")
				.IsRequired();

			builder.Property(e => e.EndAt)
				.HasColumnName("end_at")
				.IsRequired();

			builder.Property(e => e.TotalSeats)
				.HasColumnName("total_seats")
				.IsRequired();

			builder.Property(e => e.AvailableSeats)
				.HasColumnName("available_seats")
				.IsRequired();
		}
	}
#pragma warning restore CS1591
}
