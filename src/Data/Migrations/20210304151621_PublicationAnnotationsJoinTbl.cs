using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class PublicationAnnotationsJoinTbl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "PublicationAnnotations",
                columns: table => new
                {
                    PublicationsId = table.Column<int>(type: "int", nullable: false),
                    AnnotationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublicationAnnotations", x => new { x.PublicationsId, x.AnnotationsId });
                    table.ForeignKey(
                        name: "FK_PublicationAnnotations_Publications_PublicationsId",
                        column: x => x.PublicationsId,
                        principalTable: "Publications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublicationAnnotations_Annotations_AnnotationsId",
                        column: x => x.AnnotationsId,
                        principalTable: "Annotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
            name: "IX_PublicationAnnotations_AnnotationsId",
            table: "PublicationAnnotations",
            column: "AnnotationsId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublicationAnnotations");
        }
    }
}
