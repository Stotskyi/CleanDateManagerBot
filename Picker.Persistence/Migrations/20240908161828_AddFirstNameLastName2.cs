using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Picker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstNameLastName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Colivers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Colivers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Colivers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Colivers");
        }
    }
}
