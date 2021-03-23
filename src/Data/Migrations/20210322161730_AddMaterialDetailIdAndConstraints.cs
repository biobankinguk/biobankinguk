using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AddMaterialDetailIdAndConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_SampleSetId_MaterialTypeId_StorageTemperatureId_MacroscopicAssessmentId_ExtractionProcedureId_PreservationTy~",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "ExtractionProcedureId", "PreservationTypeId" },
                unique: true,
                filter: "[ExtractionProcedureId] IS NOT NULL AND [PreservationTypeId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails",
                column: "PreservationTypeId",
                principalTable: "PreservationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails");

            migrationBuilder.DropIndex(
                name: "IX_MaterialDetails_SampleSetId_MaterialTypeId_StorageTemperatureId_MacroscopicAssessmentId_ExtractionProcedureId_PreservationTy~",
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

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_PreservationTypes_PreservationTypeId",
                table: "MaterialDetails",
                column: "PreservationTypeId",
                principalTable: "PreservationTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
