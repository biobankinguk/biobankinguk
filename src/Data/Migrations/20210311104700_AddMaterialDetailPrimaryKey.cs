using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AddMaterialDetailPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

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
        }
    }
}
