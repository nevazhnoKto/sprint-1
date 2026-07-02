using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Models;

namespace Infrastructure.DataAccess.Configuration
{
#pragma warning disable CS1591
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("users");

			builder.HasKey(e => e.Id);

			builder.Property(e => e.Id)
				.HasColumnName("id")
				.ValueGeneratedNever();

			builder.Property(e => e.Login)
				.HasColumnName("login")
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(e => e.HashPassword)
				.HasColumnName("hashPassword")
				.HasMaxLength(2000);

			builder.Property(e => e.Role)
				.HasColumnName("role")
				.HasConversion<string>()
				.IsRequired();

			builder.HasMany(e => e.Bookings)
				.WithOne(b => b.User)
				.HasForeignKey(b => b.UserId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
#pragma warning restore CS1591
}
