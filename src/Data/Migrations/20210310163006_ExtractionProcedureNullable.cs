using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class ExtractionProcedureNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialTypeId",
                table: "ExtractionProcedures",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures");

            migrationBuilder.AlterColumn<int>(
                name: "MaterialTypeId",
                table: "ExtractionProcedures",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId",
                principalTable: "MaterialTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
