using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class MultipleAgeRanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CollectionSampleSets_AgeRanges_AgeRangeId",
                table: "SampleSets");

            migrationBuilder.DropIndex(
                name: "IX_CollectionSampleSets_AgeRangeId",
                table: "SampleSets");

            migrationBuilder.CreateTable(
                name: "AgeRangeSampleSet",
                columns: table => new
                {
                    AgeRangesId = table.Column<int>(type: "int", nullable: false),
                    SampleSetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgeRangeSampleSet", x => new { x.AgeRangesId, x.SampleSetsId });
                    table.ForeignKey(
                        name: "FK_AgeRangeSampleSet_AgeRanges_AgeRangesId",
                        column: x => x.AgeRangesId,
                        principalTable: "AgeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgeRangeSampleSet_SampleSets_SampleSetsId",
                        column: x => x.SampleSetsId,
                        principalTable: "SampleSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgeRangeSampleSet_SampleSetsId",
                table: "AgeRangeSampleSet",
                column: "SampleSetsId");

            //manually move over data to new many to many tables
            migrationBuilder.Sql("INSERT INTO AgeRangeSampleSet(AgeRangesId, SampleSetsId) SELECT SampleSets.AgeRangeId, SampleSets.Id FROM SampleSets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgeRangeSampleSet");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_AgeRangeId",
                table: "SampleSets",
                column: "AgeRangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CollectionSampleSets_AgeRanges_AgeRangeId",
                table: "SampleSets",
                column: "AgeRangeId",
                principalTable: "AgeRanges",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
