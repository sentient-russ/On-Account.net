using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnAccount.Migrations
{
    /// <inheritdoc />
    public partial class profile_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                schema: "Identity",
                table: "Users",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProfileImage",
                schema: "Identity",
                table: "AppUserModel",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImage",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfileImage",
                schema: "Identity",
                table: "AppUserModel");
        }
    }
}
