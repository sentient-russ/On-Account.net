using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class journalid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "transaction_1_description",
                schema: "Identity",
                table: "account");

            migrationBuilder.AddColumn<int>(
                name: "journa_id",
                schema: "Identity",
                table: "transaction",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "journa_id",
                schema: "Identity",
                table: "transaction");

            migrationBuilder.AddColumn<string>(
                name: "transaction_1_description",
                schema: "Identity",
                table: "account",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
