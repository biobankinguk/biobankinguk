using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RemoveHtaStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_HtaStatus_HtaStatusId",
                table: "Collections");

            migrationBuilder.DropTable(
                name: "HtaStatus");

            migrationBuilder.DropIndex(
                name: "IX_Collections_HtaStatusId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "HtaLicence",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "HtaStatusId",
                table: "Collections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HtaLicence",
                table: "Organisations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HtaStatusId",
                table: "Collections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HtaStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HtaStatus", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_HtaStatusId",
                table: "Collections",
                column: "HtaStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_HtaStatus_HtaStatusId",
                table: "Collections",
                column: "HtaStatusId",
                principalTable: "HtaStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
