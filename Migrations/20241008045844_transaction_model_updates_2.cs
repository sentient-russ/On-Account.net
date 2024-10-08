using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class transaction_model_updates_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "journal_id",
                schema: "Identity",
                table: "transaction");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "Identity",
                table: "transaction",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "transaction_1_description",
                schema: "Identity",
                table: "account",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "Identity",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "transaction_1_description",
                schema: "Identity",
                table: "account");

            migrationBuilder.AddColumn<int>(
                name: "journal_id",
                schema: "Identity",
                table: "transaction",
                type: "int",
                nullable: true);
        }
    }
}
