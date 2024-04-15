using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBooking.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddDisabledFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsForDisabled",
                table: "Seats",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForDisabled",
                table: "Seats");
        }
    }
}
