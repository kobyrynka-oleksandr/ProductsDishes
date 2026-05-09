using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsDishes.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class RemoveShadowDishEntityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyRations_Dishes_DishEntityId",
                table: "DailyRations");

            migrationBuilder.DropIndex(
                name: "IX_DailyRations_DishEntityId",
                table: "DailyRations");

            migrationBuilder.DropColumn(
                name: "DishEntityId",
                table: "DailyRations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DishEntityId",
                table: "DailyRations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailyRations_DishEntityId",
                table: "DailyRations",
                column: "DishEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRations_Dishes_DishEntityId",
                table: "DailyRations",
                column: "DishEntityId",
                principalTable: "Dishes",
                principalColumn: "Id");
        }
    }
}
