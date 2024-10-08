using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace oa.Migrations
{
    public partial class transactions_tbl_updates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isOpening",
                schema: "Identity",
                table: "transaction",
                newName: "is_opening");

            migrationBuilder.AlterColumn<DateTime>(
                name: "transaction_date",
                schema: "Identity",
                table: "transaction",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "debit_amount",
                schema: "Identity",
                table: "transaction",
                type: "double",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "debit_account",
                schema: "Identity",
                table: "transaction",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "credit_amount",
                schema: "Identity",
                table: "transaction",
                type: "double",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "credit_account",
                schema: "Identity",
                table: "transaction",
                type: "int",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "term",
                schema: "Identity",
                table: "account",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "current_balance",
                schema: "Identity",
                table: "account",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "is_opening",
                schema: "Identity",
                table: "transaction",
                newName: "isOpening");

            migrationBuilder.AlterColumn<int>(
                name: "transaction_date",
                schema: "Identity",
                table: "transaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<int>(
                name: "debit_amount",
                schema: "Identity",
                table: "transaction",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "debit_account",
                schema: "Identity",
                table: "transaction",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "credit_amount",
                schema: "Identity",
                table: "transaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<string>(
                name: "credit_account",
                schema: "Identity",
                table: "transaction",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                schema: "Identity",
                table: "account",
                keyColumn: "term",
                keyValue: null,
                column: "term",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "term",
                schema: "Identity",
                table: "account",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "current_balance",
                schema: "Identity",
                table: "account",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }
    }
}
