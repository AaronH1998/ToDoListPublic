using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Migrations
{
    public partial class ProjectName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Project_ProjectID",
                table: "Tasks");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "Tasks",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "Project",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Project_ProjectID",
                table: "Tasks",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Project_ProjectID",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "Project");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "Tasks",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Project_ProjectID",
                table: "Tasks",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
