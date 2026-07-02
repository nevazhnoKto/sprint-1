using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using User.Domain.Models;

namespace User.Infrastructure.DataAccess.Configuration
{
#pragma warning disable CS1591
	public class UserConfiguration : IEntityTypeConfiguration<UserModel>
	{
		public void Configure(EntityTypeBuilder<UserModel> builder)
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
		}
	}
#pragma warning restore CS1591
}
