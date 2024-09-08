using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnAccount.Migrations
{
    /// <inheritdoc />
    public partial class AcctStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AcctReinstatementDate",
                schema: "Identity",
                table: "Users",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AcctSuspensionDate",
                schema: "Identity",
                table: "Users",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcctReinstatementDate",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AcctSuspensionDate",
                schema: "Identity",
                table: "Users");
        }
    }
}
