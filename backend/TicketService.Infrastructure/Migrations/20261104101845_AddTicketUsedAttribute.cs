using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketUsedAttribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Used",
                table: "Tickets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_Email",
                table: "Passengers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_FacePersonId",
                table: "Passengers",
                column: "FacePersonId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_Email",
                table: "Passengers");

            migrationBuilder.DropIndex(
                name: "IX_Passengers_FacePersonId",
                table: "Passengers");

            migrationBuilder.DropColumn(
                name: "Used",
                table: "Tickets");
        }
    }
}
