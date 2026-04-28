using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsDishes.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddNormCoefficients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NormCoefficients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Goal = table.Column<string>(type: "text", nullable: false),
                    MinCoefficient = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxCoefficient = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NormCoefficients", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NormCoefficients");
        }
    }
}
