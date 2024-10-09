using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class journalidrev2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "journa_id",
                schema: "Identity",
                table: "transaction",
                newName: "journal_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "journal_id",
                schema: "Identity",
                table: "transaction",
                newName: "journa_id");
        }
    }
}
