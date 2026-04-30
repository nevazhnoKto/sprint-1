using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiTamakulov.Models;

namespace WebApiTamakulov.DataAccess.Configuration
{
	public class EventConfiguration : IEntityTypeConfiguration<Event>
	{
		public void Configure(EntityTypeBuilder<Event> builder)
		{
			builder.ToTable("Events");

			builder.HasKey(e => e.Id);
			builder.Property(e => e.Id).ValueGeneratedNever();

			builder.Property(e => e.Title).HasMaxLength(200).IsRequired();
			builder.Property(e => e.Description).HasMaxLength(1000);
			builder.Property(e => e.StartAt).IsRequired();
			builder.Property(e => e.EndAt).IsRequired();
			builder.Property(e => e.TotalSeats).IsRequired();
			builder.Property(e => e.AvailableSeats);

			builder.HasMany(e => e.Bookings)
				.WithOne(b => b.EventMain)
				.HasForeignKey(b => b.EventId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
