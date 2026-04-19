using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProductsDishes.DataAccess.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyRationDishWithMealType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyRationDishes_DailyRations_DailyRationsId",
                table: "DailyRationDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyRationDishes_Dishes_DishesId",
                table: "DailyRationDishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyRationDishes",
                table: "DailyRationDishes");

            migrationBuilder.RenameColumn(
                name: "DishesId",
                table: "DailyRationDishes",
                newName: "DishId");

            migrationBuilder.RenameColumn(
                name: "DailyRationsId",
                table: "DailyRationDishes",
                newName: "DailyRationId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyRationDishes_DishesId",
                table: "DailyRationDishes",
                newName: "IX_DailyRationDishes_DishId");

            migrationBuilder.AddColumn<string>(
                name: "ActivityLevel",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Sedentary");

            migrationBuilder.AddColumn<string>(
                name: "Goal",
                table: "Users",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Maintain weight");

            migrationBuilder.AddColumn<Guid>(
                name: "DishEntityId",
                table: "DailyRations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "DailyRationDishes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "MealType",
                table: "DailyRationDishes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyRationDishes",
                table: "DailyRationDishes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRations_DishEntityId",
                table: "DailyRations",
                column: "DishEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRationDishes_DailyRationId",
                table: "DailyRationDishes",
                column: "DailyRationId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRationDishes_DailyRations_DailyRationId",
                table: "DailyRationDishes",
                column: "DailyRationId",
                principalTable: "DailyRations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRationDishes_Dishes_DishId",
                table: "DailyRationDishes",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRations_Dishes_DishEntityId",
                table: "DailyRations",
                column: "DishEntityId",
                principalTable: "Dishes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyRationDishes_DailyRations_DailyRationId",
                table: "DailyRationDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyRationDishes_Dishes_DishId",
                table: "DailyRationDishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyRations_Dishes_DishEntityId",
                table: "DailyRations");

            migrationBuilder.DropIndex(
                name: "IX_DailyRations_DishEntityId",
                table: "DailyRations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyRationDishes",
                table: "DailyRationDishes");

            migrationBuilder.DropIndex(
                name: "IX_DailyRationDishes_DailyRationId",
                table: "DailyRationDishes");

            migrationBuilder.DropColumn(
                name: "ActivityLevel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Goal",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DishEntityId",
                table: "DailyRations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "DailyRationDishes");

            migrationBuilder.DropColumn(
                name: "MealType",
                table: "DailyRationDishes");

            migrationBuilder.RenameColumn(
                name: "DishId",
                table: "DailyRationDishes",
                newName: "DishesId");

            migrationBuilder.RenameColumn(
                name: "DailyRationId",
                table: "DailyRationDishes",
                newName: "DailyRationsId");

            migrationBuilder.RenameIndex(
                name: "IX_DailyRationDishes_DishId",
                table: "DailyRationDishes",
                newName: "IX_DailyRationDishes_DishesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyRationDishes",
                table: "DailyRationDishes",
                columns: new[] { "DailyRationsId", "DishesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRationDishes_DailyRations_DailyRationsId",
                table: "DailyRationDishes",
                column: "DailyRationsId",
                principalTable: "DailyRations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRationDishes_Dishes_DishesId",
                table: "DailyRationDishes",
                column: "DishesId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
