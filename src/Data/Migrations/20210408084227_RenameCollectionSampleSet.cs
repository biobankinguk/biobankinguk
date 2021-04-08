using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RenameCollectionSampleSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_CollectionSampleSets_CollectionSampleSetSampleSetId",
                table: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "CollectionSampleSets");

            migrationBuilder.DropIndex(
                name: "IX_MaterialDetails_CollectionSampleSetSampleSetId",
                table: "MaterialDetails");

            migrationBuilder.DropColumn(
                name: "CollectionSampleSetSampleSetId",
                table: "MaterialDetails");

            migrationBuilder.CreateTable(
                name: "SampleSets",
                columns: table => new
                {
                    SampleSetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    SexId = table.Column<int>(type: "int", nullable: false),
                    AgeRangeId = table.Column<int>(type: "int", nullable: false),
                    DonorCountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleSets", x => x.SampleSetId);
                    table.ForeignKey(
                        name: "FK_SampleSets_AgeRanges_AgeRangeId",
                        column: x => x.AgeRangeId,
                        principalTable: "AgeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_DonorCounts_DonorCountId",
                        column: x => x.DonorCountId,
                        principalTable: "DonorCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleSets_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_AgeRangeId",
                table: "SampleSets",
                column: "AgeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_CollectionId",
                table: "SampleSets",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_DonorCountId",
                table: "SampleSets",
                column: "DonorCountId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleSets_SexId",
                table: "SampleSets",
                column: "SexId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails",
                column: "SampleSetId",
                principalTable: "SampleSets",
                principalColumn: "SampleSetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_SampleSets_SampleSetId",
                table: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "SampleSets");

            migrationBuilder.AddColumn<int>(
                name: "CollectionSampleSetSampleSetId",
                table: "MaterialDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CollectionSampleSets",
                columns: table => new
                {
                    SampleSetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgeRangeId = table.Column<int>(type: "int", nullable: false),
                    CollectionId = table.Column<int>(type: "int", nullable: false),
                    DonorCountId = table.Column<int>(type: "int", nullable: false),
                    SexId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionSampleSets", x => x.SampleSetId);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_AgeRanges_AgeRangeId",
                        column: x => x.AgeRangeId,
                        principalTable: "AgeRanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "CollectionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_DonorCounts_DonorCountId",
                        column: x => x.DonorCountId,
                        principalTable: "DonorCounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CollectionSampleSets_Sexes_SexId",
                        column: x => x.SexId,
                        principalTable: "Sexes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_CollectionSampleSetSampleSetId",
                table: "MaterialDetails",
                column: "CollectionSampleSetSampleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_AgeRangeId",
                table: "CollectionSampleSets",
                column: "AgeRangeId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_CollectionId",
                table: "CollectionSampleSets",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_DonorCountId",
                table: "CollectionSampleSets",
                column: "DonorCountId");

            migrationBuilder.CreateIndex(
                name: "IX_CollectionSampleSets_SexId",
                table: "CollectionSampleSets",
                column: "SexId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_CollectionSampleSets_CollectionSampleSetSampleSetId",
                table: "MaterialDetails",
                column: "CollectionSampleSetSampleSetId",
                principalTable: "CollectionSampleSets",
                principalColumn: "SampleSetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
