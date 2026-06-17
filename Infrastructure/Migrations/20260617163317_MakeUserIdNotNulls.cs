using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class MakeUserIdNotNulls : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// Сделать столбец NOT NULL
			migrationBuilder.AlterColumn<Guid>(
				name: "UserId",
				table: "bookings",
				type: "uuid",
				nullable: false);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<Guid>(
		  name: "UserId",
		  table: "bookings",
		  type: "uuid",
		  nullable: true);
		}
	}
}
