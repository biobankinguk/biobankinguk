using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Biobanks.Data.Migrations
{
    public partial class AddMaterialDetailPrimaryKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #region EF6 Schema Migrations
            Console.WriteLine(@"
            Note:
            Before applying this migration. Ensure that all records in MaterialDetails
            have a non-null FK value for PreservationTypeId and ExtractionProcedureId
            otherwise this mirgation WILL fail.
            
            Press any key to continue, or Ctrl+C to abort this migration.");
            Console.ReadKey();

            // Rename PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_dbo.MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId" });
            #endregion

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AlterColumn<int>(
                name: "PreservationTypeId",
                table: "MaterialDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "ExtractionProcedureId", "PreservationTypeId" });

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

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId" });

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
