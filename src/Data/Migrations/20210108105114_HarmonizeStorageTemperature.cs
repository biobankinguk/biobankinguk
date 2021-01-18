using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class HarmonizeStorageTemperature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "StorageTemperatures",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "StorageTemperatures");
        }
    }
}
