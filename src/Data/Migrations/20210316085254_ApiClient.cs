using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class ApiClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiClients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientSecretHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiClientOrganisation",
                columns: table => new
                {
                    ApiClientsId = table.Column<int>(type: "int", nullable: false),
                    OrganisationsOrganisationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiClientOrganisation", x => new { x.ApiClientsId, x.OrganisationsOrganisationId });
                    table.ForeignKey(
                        name: "FK_ApiClientOrganisation_ApiClients_ApiClientsId",
                        column: x => x.ApiClientsId,
                        principalTable: "ApiClients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiClientOrganisation_Organisations_OrganisationsOrganisationId",
                        column: x => x.OrganisationsOrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "OrganisationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiClientOrganisation_OrganisationsOrganisationId",
                table: "ApiClientOrganisation",
                column: "OrganisationsOrganisationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiClientOrganisation");

            migrationBuilder.DropTable(
                name: "ApiClients");
        }
    }
}
