using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnAccount.Migrations
{
    /// <inheritdoc />
    public partial class PasswordResetDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordResetDays",
                schema: "Identity",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordResetDays",
                schema: "Identity",
                table: "Users");
        }
    }
}
