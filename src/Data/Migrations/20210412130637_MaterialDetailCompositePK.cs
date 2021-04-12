using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class MaterialDetailCompositePK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "MaterialDetails",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                columns: new[] { "Id", "SampleSetId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "MaterialDetails",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MaterialDetails",
                table: "MaterialDetails",
                column: "Id");
        }
    }
}
