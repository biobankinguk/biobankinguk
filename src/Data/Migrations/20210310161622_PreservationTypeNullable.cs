using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class PreservationTypeNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                table: "PreservationTypes");

            migrationBuilder.AlterColumn<int>(
                name: "StorageTemperatureId",
                table: "PreservationTypes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                table: "PreservationTypes",
                column: "StorageTemperatureId",
                principalTable: "StorageTemperatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                table: "PreservationTypes");

            migrationBuilder.AlterColumn<int>(
                name: "StorageTemperatureId",
                table: "PreservationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                table: "PreservationTypes",
                column: "StorageTemperatureId",
                principalTable: "StorageTemperatures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
