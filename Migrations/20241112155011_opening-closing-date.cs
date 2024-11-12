using System;
using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable

namespace oa.Migrations
{
    public partial class openingclosingdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "closing_user",
                schema: "Identity",
                table: "system_settings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "open_close_date",
                schema: "Identity",
                table: "system_settings",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "open_close_on_date",
                schema: "Identity",
                table: "system_settings",
                type: "datetime(6)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "closing_user",
                schema: "Identity",
                table: "system_settings");

            migrationBuilder.DropColumn(
                name: "open_close_date",
                schema: "Identity",
                table: "system_settings");

            migrationBuilder.DropColumn(
                name: "open_close_on_date",
                schema: "Identity",
                table: "system_settings");
        }
    }
}
