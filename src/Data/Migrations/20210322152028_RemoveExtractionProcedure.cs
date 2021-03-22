using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RemoveExtractionProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "ExtractionProcedures");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "nvarchar(20)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "PreservationTypeId" });

            migrationBuilder.CreateTable(
                name: "MaterialTypeOntologyTerm",
                columns: table => new
                {
                    ExtractionProceduresId = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    MaterialTypesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypeOntologyTerm", x => new { x.ExtractionProceduresId, x.MaterialTypesId });
                    table.ForeignKey(
                        name: "FK_MaterialTypeOntologyTerm_MaterialTypes_MaterialTypesId",
                        column: x => x.MaterialTypesId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MaterialTypeOntologyTerm_OntologyTerms_ExtractionProceduresId",
                        column: x => x.ExtractionProceduresId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialTypeOntologyTerm_MaterialTypesId",
                table: "MaterialTypeOntologyTerm",
                column: "MaterialTypesId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_OntologyTerms_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "OntologyTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_OntologyTerms_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "MaterialTypeOntologyTerm");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            migrationBuilder.AlterColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId", "ExtractionProcedureId", "PreservationTypeId" });

            migrationBuilder.CreateTable(
                name: "ExtractionProcedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDefaultValue = table.Column<bool>(type: "bit", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractionProcedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtractionProcedures_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "ExtractionProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
