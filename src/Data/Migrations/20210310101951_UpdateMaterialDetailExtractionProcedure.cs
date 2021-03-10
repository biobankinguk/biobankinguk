using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class UpdateMaterialDetailExtractionProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "ExtractionProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropIndex(
                name: "IX_MaterialDetails_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropColumn(
                name: "ExtractionProcedureId",
                table: "MaterialDetails");
        }
    }
}
