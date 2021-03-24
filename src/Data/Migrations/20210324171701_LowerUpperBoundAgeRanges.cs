using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class LowerUpperBoundAgeRanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LowerBound",
                table: "AgeRanges",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpperBound",
                table: "AgeRanges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgeRanges_LowerBound_UpperBound",
                table: "AgeRanges",
                columns: new[] { "LowerBound", "UpperBound" },
                unique: true,
                filter: "[LowerBound] IS NOT NULL AND [UpperBound] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_ONLY_ONE_NULL",
                table: "AgeRanges",
                sql: "[LowerBound] IS NOT NULL OR [UpperBound] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AgeRanges_LowerBound_UpperBound",
                table: "AgeRanges");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ONLY_ONE_NULL",
                table: "AgeRanges");

            migrationBuilder.DropColumn(
                name: "LowerBound",
                table: "AgeRanges");

            migrationBuilder.DropColumn(
                name: "UpperBound",
                table: "AgeRanges");
        }
    }
}
