using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RenameSampleSetPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_CollectionSampleSets_CollectionSampleSetSampleSetId",
                table: "MaterialDetails");

            migrationBuilder.RenameColumn(
                name: "SampleSetId",
                table: "SampleSets",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails",
                column: "SampleSetId",
                principalTable: "SampleSets",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "SampleSets",
                newName: "SampleSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_CollectionSampleSets_CollectionSampleSetSampleSetId",
                table: "MaterialDetails",
                column: "SampleSetSampleSetId",
                principalTable: "SampleSets",
                principalColumn: "SampleSetId");
        }
    }
}
