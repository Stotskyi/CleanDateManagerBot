using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakenullableforeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colivers_CleaningTimes_CleaningTimeId",
                table: "Colivers");

            migrationBuilder.AlterColumn<int>(
                name: "CleaningTimeId",
                table: "Colivers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Colivers_CleaningTimes_CleaningTimeId",
                table: "Colivers",
                column: "CleaningTimeId",
                principalTable: "CleaningTimes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colivers_CleaningTimes_CleaningTimeId",
                table: "Colivers");

            migrationBuilder.AlterColumn<int>(
                name: "CleaningTimeId",
                table: "Colivers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Colivers_CleaningTimes_CleaningTimeId",
                table: "Colivers",
                column: "CleaningTimeId",
                principalTable: "CleaningTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
