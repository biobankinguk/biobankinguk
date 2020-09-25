using Microsoft.EntityFrameworkCore.Migrations;

namespace Publications.Migrations
{
    public partial class ChangePublicationIdToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicationId",
                table: "Publications",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PublicationId",
                table: "Publications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
