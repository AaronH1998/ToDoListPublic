using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Migrations.SuggestionDb
{
    public partial class RequiredDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Suggestions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "Suggestions",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
