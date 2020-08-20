using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Analytics.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    PagePath = table.Column<string>(nullable: true),
                    PreviousPagePath = table.Column<string>(nullable: true),
                    Segment = table.Column<string>(nullable: true),
                    Source = table.Column<string>(nullable: true),
                    Hostname = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Counts = table.Column<int>(nullable: false),
                    OrganisationExternalId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAnalytics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationAnalytics");
        }
    }
}
