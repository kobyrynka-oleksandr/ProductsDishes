using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductsDishes.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddNormCoefficientConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "NormCoefficients",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "NormCoefficients",
                columns: new[] { "Id", "Goal", "MaxCoefficient", "MinCoefficient" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000001"), "Lose weight", 1.05m, 0.85m },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000002"), "Maintain weight", 1.10m, 0.90m },
                    { new Guid("aaaaaaaa-0000-0000-0000-000000000003"), "Gain weight", 1.15m, 0.95m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "NormCoefficients",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "NormCoefficients",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "NormCoefficients",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-0000-0000-0000-000000000003"));

            migrationBuilder.AlterColumn<string>(
                name: "Goal",
                table: "NormCoefficients",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
