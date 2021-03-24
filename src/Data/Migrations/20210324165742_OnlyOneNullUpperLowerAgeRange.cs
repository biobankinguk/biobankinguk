using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class OnlyOneNullUpperLowerAgeRange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_ONLY_ONE_NULL",
                table: "AgeRanges",
                sql: "[LowerBound] IS NOT NULL OR [UpperBound] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_ONLY_ONE_NULL",
                table: "AgeRanges");
        }
    }
}
