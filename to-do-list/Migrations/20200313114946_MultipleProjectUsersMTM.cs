using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Migrations
{
    public partial class MultipleProjectUsersMTM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Project_ProjectID",
                table: "ProjectUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_ProjectID",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "ProjectUserID",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ProjectUsers");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "ProjectUsers",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "ProjectUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                columns: new[] { "ProjectID", "UserID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "ProjectUsers");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectID",
                table: "ProjectUsers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ProjectUserID",
                table: "ProjectUsers",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ProjectUsers",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                column: "ProjectUserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_ProjectID",
                table: "ProjectUsers",
                column: "ProjectID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Project_ProjectID",
                table: "ProjectUsers",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
