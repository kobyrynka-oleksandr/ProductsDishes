using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsDishes.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexOnGoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NormCoefficients_Goal",
                table: "NormCoefficients",
                column: "Goal",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NormCoefficients_Goal",
                table: "NormCoefficients");
        }
    }
}
