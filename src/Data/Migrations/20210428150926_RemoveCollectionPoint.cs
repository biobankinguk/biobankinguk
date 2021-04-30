using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class RemoveCollectionPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collections_CollectionPoints_CollectionPointId",
                table: "Collections");

            migrationBuilder.DropTable(
                name: "CollectionPoints");

            migrationBuilder.DropIndex(
                name: "IX_Collections_CollectionPointId",
                table: "Collections");

            migrationBuilder.DropColumn(
                name: "CollectionPointId",
                table: "Collections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollectionPointId",
                table: "Collections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CollectionPoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionPoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_CollectionPointId",
                table: "Collections",
                column: "CollectionPointId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collections_CollectionPoints_CollectionPointId",
                table: "Collections",
                column: "CollectionPointId",
                principalTable: "CollectionPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
