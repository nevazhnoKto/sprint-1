using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
	/// <inheritdoc />
	public partial class addusers : Migration
	{
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<Guid>(
				name: "UserId",
				table: "bookings",
				type: "uuid",
				nullable: true
				);

			migrationBuilder.CreateTable(
				name: "users",
				columns: table => new
				{
					id = table.Column<Guid>(type: "uuid", nullable: false),
					login = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
					hashPassword = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
					role = table.Column<string>(type: "text", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_users", x => x.id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_bookings_UserId",
				table: "bookings",
				column: "UserId");

			migrationBuilder.AddForeignKey(
				name: "FK_bookings_users_UserId",
				table: "bookings",
				column: "UserId",
				principalTable: "users",
				principalColumn: "id",
				onDelete: ReferentialAction.Cascade);
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_bookings_users_UserId",
				table: "bookings");

			migrationBuilder.DropTable(
				name: "users");

			migrationBuilder.DropIndex(
				name: "IX_bookings_UserId",
				table: "bookings");

			migrationBuilder.DropColumn(
				name: "UserId",
				table: "bookings");
		}
	}
}
