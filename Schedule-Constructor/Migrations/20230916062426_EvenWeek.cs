using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Schedule_Constructor.Migrations
{
    /// <inheritdoc />
    public partial class EvenWeek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEvenWeek",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEvenWeek",
                table: "Schedules");
        }
    }
}
