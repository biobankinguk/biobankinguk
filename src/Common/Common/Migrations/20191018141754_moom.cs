using Microsoft.EntityFrameworkCore.Migrations;

namespace Common.Migrations
{
    public partial class moom : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialType",
                table: "MaterialTypeGroupMaterialType");

            migrationBuilder.RenameTable(
                name: "MaterialTypeGroupMaterialType",
                newName: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes",
                newName: "IX_MaterialTypeGroupMaterialTypes_MaterialTypeGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialTypes",
                table: "MaterialTypeGroupMaterialTypes",
                columns: new[] { "MaterialTypeId", "MaterialTypeGroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes",
                column: "MaterialTypeGroupId",
                principalTable: "MaterialTypeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialTypes",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.DropForeignKey(
                name: "FK_MaterialTypeGroupMaterialTypes_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialTypes",
                table: "MaterialTypeGroupMaterialTypes");

            migrationBuilder.RenameTable(
                name: "MaterialTypeGroupMaterialTypes",
                newName: "MaterialTypeGroupMaterialType");

            migrationBuilder.RenameIndex(
                name: "IX_MaterialTypeGroupMaterialTypes_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                newName: "IX_MaterialTypeGroupMaterialType_MaterialTypeGroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialTypeGroupMaterialType",
                table: "MaterialTypeGroupMaterialType",
                columns: new[] { "MaterialTypeId", "MaterialTypeGroupId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypeGroups_MaterialTypeGroupId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeGroupId",
                principalTable: "MaterialTypeGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialTypeGroupMaterialType_MaterialTypes_MaterialTypeId",
                table: "MaterialTypeGroupMaterialType",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
