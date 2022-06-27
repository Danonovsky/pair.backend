using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations
{
    public partial class FixNoUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserDtoId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserDto",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Applications_UserDtoId",
                table: "Applications",
                column: "UserDtoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_UserDto_UserDtoId",
                table: "Applications",
                column: "UserDtoId",
                principalTable: "UserDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_UserDto_UserDtoId",
                table: "Applications");

            migrationBuilder.DropTable(
                name: "UserDto");

            migrationBuilder.DropIndex(
                name: "IX_Applications_UserDtoId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "UserDtoId",
                table: "Applications");
        }
    }
}
