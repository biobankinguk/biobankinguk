using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AddMaterialDetailPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

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

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultValue",
                table: "PreservationTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PreservationTypeId",
                table: "MaterialDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MaterialTypeId",
                table: "ExtractionProcedures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultValue",
                table: "ExtractionProcedures",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "ExtractionProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails",
                column: "PreservationTypeId",
                principalTable: "PreservationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PreservationTypes_StorageTemperatures_StorageTemperatureId",
                table: "PreservationTypes");

            migrationBuilder.DropColumn(
                name: "IsDefaultValue",
                table: "PreservationTypes");

            migrationBuilder.DropColumn(
                name: "IsDefaultValue",
                table: "ExtractionProcedures");

            migrationBuilder.AlterColumn<int>(
                name: "StorageTemperatureId",
                table: "PreservationTypes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PreservationTypeId",
                table: "MaterialDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialTypeId",
                table: "ExtractionProcedures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "ExtractionProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails",
                column: "PreservationTypeId",
                principalTable: "PreservationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
