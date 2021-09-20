using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class ReferenceDataSortOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Status ID Change To Identity
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Statuses_StatusId",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Statuses");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Statuses",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Statuses_StatusId",
                table: "Submissions",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            // Rename Annotations Name to Value
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Annotations",
                newName: "Value");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Annotations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Migrations To ReferenceDataBase
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "TreatmentLocations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Statuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Statuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "SnomedTags",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "SampleContentMethods",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "RegistrationReasons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "RegistrationReasons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "OntologyVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Ontologies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "MaterialTypeGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Funders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Funders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Countries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Counties",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Counties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AssociatedDataTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AssociatedDataTypeGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AnnualStatistics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AnnualStatistics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AnnualStatisticGroups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "AnnualStatisticGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Annotations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert Statuses Identity Id
            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Statuses_StatusId",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Statuses");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Statuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("DECLARE @i INT = 0; UPDATE Statuses SET Id = @i, @i = @i+1;");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statuses",
                table: "Statuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Statuses_StatusId",
                table: "Submissions",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            // Revert Annotations Value Column To Name 
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Annotations",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Annotations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            // Revert Changes For ReferenceDataBase
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "TreatmentLocations");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Statuses");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "SnomedTags");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "SampleContentMethods");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "RegistrationReasons");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "OntologyVersions");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Ontologies");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "MaterialTypeGroups");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Funders");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Counties");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AssociatedDataTypes");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AssociatedDataTypeGroups");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AnnualStatistics");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "AnnualStatisticGroups");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Annotations");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Statuses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "RegistrationReasons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Funders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Counties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AnnualStatistics",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "AnnualStatisticGroups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        }
    }
}
