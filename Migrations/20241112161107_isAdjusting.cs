using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class isAdjusting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "is_adjusting",
                schema: "Identity",
                table: "transaction",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_adjusting",
                schema: "Identity",
                table: "transaction");
        }
    }
}
