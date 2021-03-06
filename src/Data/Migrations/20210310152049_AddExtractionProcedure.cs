﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class AddExtractionProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtractionProcedureId",
                table: "MaterialDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExtractionProcedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtractionProcedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtractionProcedures_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialDetails_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_ExtractionProcedures_MaterialTypeId",
                table: "ExtractionProcedures",
                column: "MaterialTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails",
                column: "ExtractionProcedureId",
                principalTable: "ExtractionProcedures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialDetails_ExtractionProcedures_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropTable(
                name: "ExtractionProcedures");

            migrationBuilder.DropIndex(
                name: "IX_MaterialDetails_ExtractionProcedureId",
                table: "MaterialDetails");

            migrationBuilder.DropColumn(
                name: "ExtractionProcedureId",
                table: "MaterialDetails");
        }
    }
}
