using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCycleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "Cycle",
                table: "CleaningTimes",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cycle",
                table: "CleaningTimes");
        }
    }
}
