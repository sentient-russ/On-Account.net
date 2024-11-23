using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class system_start_date : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "transaction",
                keyColumn: "is_adjusting",
                keyValue: null,
                column: "is_adjusting",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "is_adjusting",
                schema: "Identity",
                table: "transaction",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "system_start_date",
                schema: "Identity",
                table: "system_settings",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "system_start_date",
                schema: "Identity",
                table: "system_settings");

            migrationBuilder.AlterColumn<string>(
                name: "is_adjusting",
                schema: "Identity",
                table: "transaction",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
