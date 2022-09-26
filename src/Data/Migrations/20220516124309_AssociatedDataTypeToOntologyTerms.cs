using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AssociatedDataTypeToOntologyTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssociatedDataTypeOntologyTerm",
                columns: table => new
                {
                    AssociatedDataTypesId = table.Column<int>(type: "int", nullable: false),
                    OntologyTermsId = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssociatedDataTypeOntologyTerm", x => new { x.AssociatedDataTypesId, x.OntologyTermsId });
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypeOntologyTerm_AssociatedDataTypes_AssociatedDataTypesId",
                        column: x => x.AssociatedDataTypesId,
                        principalTable: "AssociatedDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssociatedDataTypeOntologyTerm_OntologyTerms_OntologyTermsId",
                        column: x => x.OntologyTermsId,
                        principalTable: "OntologyTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssociatedDataTypeOntologyTerm_OntologyTermsId",
                table: "AssociatedDataTypeOntologyTerm",
                column: "OntologyTermsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssociatedDataTypeOntologyTerm");
        }
    }
}
