using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class LowerUpperBoundAgeRanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LowerBound",
                table: "AgeRanges",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperBound",
                table: "AgeRanges",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AgeRanges_LowerBound_UpperBound",
                table: "AgeRanges",
                columns: new[] { "LowerBound", "UpperBound" },
                unique: true,
                filter: "[LowerBound] IS NOT NULL AND [UpperBound] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AgeRanges_LowerBound_UpperBound",
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
