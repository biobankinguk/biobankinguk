using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations
{
    public partial class TokenLogDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "TokenRecords",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "TokenRecords");
        }
    }
}
