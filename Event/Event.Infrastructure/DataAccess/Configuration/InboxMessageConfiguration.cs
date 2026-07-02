using Event.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Event.Infrastructure.DataAccess.Configuration
{
#pragma warning disable CS1591
	public class InboxMessageConfiguration : IEntityTypeConfiguration<InboxMessage>
	{
		public void Configure(EntityTypeBuilder<InboxMessage> builder)
		{
			builder.ToTable("inbox_messages");

			builder.HasKey(e => new { e.Id, e.MessageName });

			builder.Property(e => e.Id)
				.HasColumnName("id_booking")
				.ValueGeneratedNever();

			builder.Property(e => e.MessageName)
				.HasColumnName("message")
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(e => e.ProcessedAt)
				.HasColumnName("create_date")
				.IsRequired();
		}
	}
#pragma warning restore CS1591
}
