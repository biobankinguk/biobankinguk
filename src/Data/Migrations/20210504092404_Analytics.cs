using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class Analytics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DirectoryAnalyticEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventCategory = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventAction = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Biobank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Segment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Counts = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAnalyticEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryAnalyticMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PagePathLevel1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Segment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sessions = table.Column<int>(type: "int", nullable: false),
                    BounceRate = table.Column<int>(type: "int", nullable: false),
                    PercentNewSessions = table.Column<int>(type: "int", nullable: false),
                    AvgSessionDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryAnalyticMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationAnalytics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousPagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Segment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hostname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Counts = table.Column<int>(type: "int", nullable: false),
                    OrganisationExternalId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationAnalytics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryAnalyticEvents");

            migrationBuilder.DropTable(
                name: "DirectoryAnalyticMetrics");

            migrationBuilder.DropTable(
                name: "OrganisationAnalytics");
        }
    }
}
