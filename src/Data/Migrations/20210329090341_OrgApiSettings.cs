using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class OrgApiSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessConditionId",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CollectionTypeId",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_AccessConditionId",
                table: "Organisations",
                column: "AccessConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_CollectionTypeId",
                table: "Organisations",
                column: "CollectionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_AccessConditions_AccessConditionId",
                table: "Organisations",
                column: "AccessConditionId",
                principalTable: "AccessConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_CollectionTypes_CollectionTypeId",
                table: "Organisations",
                column: "CollectionTypeId",
                principalTable: "CollectionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_AccessConditions_AccessConditionId",
                table: "Organisations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_CollectionTypes_CollectionTypeId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_AccessConditionId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_CollectionTypeId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "AccessConditionId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "CollectionTypeId",
                table: "Organisations");
        }
    }
}
