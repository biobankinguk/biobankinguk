using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class SampleSetCascadeDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails",
                column: "SampleSetId",
                principalTable: "SampleSets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails",
                column: "SampleSetId",
                principalTable: "SampleSets",
                principalColumn: "Id");
        }
    }
}
