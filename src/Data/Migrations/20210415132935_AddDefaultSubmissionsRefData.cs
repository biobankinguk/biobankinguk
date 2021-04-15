using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AddDefaultSubmissionsRefData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultSubmissionsAccessConditionId",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultSubmissionsCollectionTypeId",
                table: "Organisations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_DefaultSubmissionsAccessConditionId",
                table: "Organisations",
                column: "DefaultSubmissionsAccessConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_DefaultSubmissionsCollectionTypeId",
                table: "Organisations",
                column: "DefaultSubmissionsCollectionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_AccessConditions_DefaultSubmissionsAccessConditionId",
                table: "Organisations",
                column: "DefaultSubmissionsAccessConditionId",
                principalTable: "AccessConditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_CollectionTypes_DefaultSubmissionsCollectionTypeId",
                table: "Organisations",
                column: "DefaultSubmissionsCollectionTypeId",
                principalTable: "CollectionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_AccessConditions_DefaultSubmissionsAccessConditionId",
                table: "Organisations");

            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_CollectionTypes_DefaultSubmissionsCollectionTypeId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_DefaultSubmissionsAccessConditionId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_DefaultSubmissionsCollectionTypeId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "DefaultSubmissionsAccessConditionId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "DefaultSubmissionsCollectionTypeId",
                table: "Organisations");
        }
    }
}
