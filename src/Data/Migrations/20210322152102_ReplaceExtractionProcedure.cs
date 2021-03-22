using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class ReplaceExtractionProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "ExtractionProcedureId", "PreservationTypeId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "PreservationTypeId" });
        }
    }
}
