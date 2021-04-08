using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RenameCollectionSampleSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "CollectionSampleSets", schema: "dbo", newName: "SampleSets", newSchema: "dbo");

            migrationBuilder.RenameColumn(name: "CollectionSampleSetSampleSetId", table: "MaterialDetails", newName: "SampleSetSampleSetId", schema: "dbo");            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(name: "SampleSets", schema: "dbo", newName: "CollectionSampleSets", newSchema: "dbo");

            migrationBuilder.RenameColumn(name: "SampleSetSampleSetId", table: "MaterialDetails", newName: "CollectionSampleSetSampleSetId", schema: "dbo");
        }
    }
}
