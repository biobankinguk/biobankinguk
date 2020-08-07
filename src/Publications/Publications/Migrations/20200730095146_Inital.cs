using Microsoft.EntityFrameworkCore.Migrations;

namespace Publications.Migrations
{
    public partial class Inital : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Publications",
                columns: table => new
                {
                    InternalId = table.Column<int>(nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                    PublicationId = table.Column<int>(nullable: false),
                    Organisation = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Authors = table.Column<string>(nullable: true),
                    Journal = table.Column<string>(nullable: true),
                    Year = table.Column<int>(nullable: false),
                    DOI = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publications", x => x.InternalId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Publications");
        }
    }
}
