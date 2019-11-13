using Microsoft.EntityFrameworkCore.Migrations;

namespace Upload.Common.Migrations
{
    public partial class biobanktoorg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_OmopTerm_DiagnosisCodeId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_StagedDiagnoses_OmopTerm_DiagnosisCodeId",
                table: "StagedDiagnoses");

            migrationBuilder.DropTable(
                name: "OmopTerm");

            migrationBuilder.DropIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples");

            migrationBuilder.AlterColumn<string>(
                name: "UploadStatus",
                table: "Submissions",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SampleContentId",
                table: "StagedSamples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionSiteId",
                table: "StagedSamples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionProcedureId",
                table: "StagedSamples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollectionName",
                table: "StagedSamples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SampleContentId",
                table: "Samples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionSiteId",
                table: "Samples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionProcedureId",
                table: "Samples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CollectionName",
                table: "Samples",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RecordIdentifiers",
                table: "Errors",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Errors",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OmopTermDto",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    TagId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OmopTermDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_OmopTermDto_DiagnosisCodeId",
                table: "Diagnoses",
                column: "DiagnosisCodeId",
                principalTable: "OmopTermDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StagedDiagnoses_OmopTermDto_DiagnosisCodeId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeId",
                principalTable: "OmopTermDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_OmopTermDto_DiagnosisCodeId",
                table: "Diagnoses");

            migrationBuilder.DropForeignKey(
                name: "FK_StagedDiagnoses_OmopTermDto_DiagnosisCodeId",
                table: "StagedDiagnoses");

            migrationBuilder.DropTable(
                name: "OmopTermDto");

            migrationBuilder.DropIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples");

            migrationBuilder.DropIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples");

            migrationBuilder.AlterColumn<string>(
                name: "UploadStatus",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SampleContentId",
                table: "StagedSamples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionSiteId",
                table: "StagedSamples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionProcedureId",
                table: "StagedSamples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "CollectionName",
                table: "StagedSamples",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SampleContentId",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionSiteId",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ExtractionProcedureId",
                table: "Samples",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "CollectionName",
                table: "Samples",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RecordIdentifiers",
                table: "Errors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Errors",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "OmopTerm",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TagId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OmopTerm", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StagedSamples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "StagedSamples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Samples_OrganisationId_IndividualReferenceId_Barcode_CollectionName",
                table: "Samples",
                columns: new[] { "OrganisationId", "IndividualReferenceId", "Barcode", "CollectionName" },
                unique: true,
                filter: "[CollectionName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_OmopTerm_DiagnosisCodeId",
                table: "Diagnoses",
                column: "DiagnosisCodeId",
                principalTable: "OmopTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StagedDiagnoses_OmopTerm_DiagnosisCodeId",
                table: "StagedDiagnoses",
                column: "DiagnosisCodeId",
                principalTable: "OmopTerm",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
