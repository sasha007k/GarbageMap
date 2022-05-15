using Microsoft.EntityFrameworkCore.Migrations;

namespace GarbageMap.Migrations
{
    public partial class AddOrganizationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "CameraPlaces",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CameraPlaces_OrganizationId",
                table: "CameraPlaces",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_CameraPlaces_AspNetUsers_OrganizationId",
                table: "CameraPlaces",
                column: "OrganizationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CameraPlaces_AspNetUsers_OrganizationId",
                table: "CameraPlaces");

            migrationBuilder.DropIndex(
                name: "IX_CameraPlaces_OrganizationId",
                table: "CameraPlaces");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "CameraPlaces");
        }
    }
}
