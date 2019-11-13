using Microsoft.EntityFrameworkCore.Migrations;

namespace Upload.Common.Migrations
{
    public partial class StatusChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_UploadStatuses_UploadStatusId",
                table: "Submissions");

            migrationBuilder.DropTable(
                name: "UploadStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Submissions_UploadStatusId",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "UploadStatusId",
                table: "Submissions");

            migrationBuilder.AddColumn<string>(
                name: "UploadStatus",
                table: "Submissions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadStatus",
                table: "Submissions");

            migrationBuilder.AddColumn<int>(
                name: "UploadStatusId",
                table: "Submissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UploadStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_UploadStatusId",
                table: "Submissions",
                column: "UploadStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_UploadStatuses_UploadStatusId",
                table: "Submissions",
                column: "UploadStatusId",
                principalTable: "UploadStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
