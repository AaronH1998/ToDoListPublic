using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoList.Migrations
{
    public partial class MTMRelationshipAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProjectUsers_UserID",
                table: "ProjectUsers",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Project_ProjectID",
                table: "ProjectUsers",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "ProjectID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UserID",
                table: "ProjectUsers",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Project_ProjectID",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UserID",
                table: "ProjectUsers");

            migrationBuilder.DropIndex(
                name: "IX_ProjectUsers_UserID",
                table: "ProjectUsers");
        }
    }
}
