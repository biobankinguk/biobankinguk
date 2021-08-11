using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class Capabilities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CapabilityAssociatedDatas_DiagnosisCapabilities_DiagnosisCapabilityId",
                table: "CapabilityAssociatedDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisCapabilities_OntologyTerms_OntologyTermId",
                table: "DiagnosisCapabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisCapabilities_Organisations_OrganisationId",
                table: "DiagnosisCapabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisCapabilities_SampleCollectionModes_SampleCollectionModeId",
                table: "DiagnosisCapabilities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DiagnosisCapabilities",
                table: "DiagnosisCapabilities");

            migrationBuilder.RenameTable(
                name: "DiagnosisCapabilities",
                newName: "Capabilities");

            migrationBuilder.RenameColumn(
                name: "DiagnosisCapabilityId",
                table: "Capabilities",
                newName: "CapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_DiagnosisCapabilities_SampleCollectionModeId",
                table: "Capabilities",
                newName: "IX_Capabilities_SampleCollectionModeId");

            migrationBuilder.RenameIndex(
                name: "IX_DiagnosisCapabilities_OrganisationId",
                table: "Capabilities",
                newName: "IX_Capabilities_OrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_DiagnosisCapabilities_OntologyTermId",
                table: "Capabilities",
                newName: "IX_Capabilities_OntologyTermId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Capabilities",
                table: "Capabilities",
                column: "CapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Capabilities_OntologyTerms_OntologyTermId",
                table: "Capabilities",
                column: "OntologyTermId",
                principalTable: "OntologyTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Capabilities_Organisations_OrganisationId",
                table: "Capabilities",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "OrganisationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Capabilities_SampleCollectionModes_SampleCollectionModeId",
                table: "Capabilities",
                column: "SampleCollectionModeId",
                principalTable: "SampleCollectionModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CapabilityAssociatedDatas_Capabilities_DiagnosisCapabilityId",
                table: "CapabilityAssociatedDatas",
                column: "DiagnosisCapabilityId",
                principalTable: "Capabilities",
                principalColumn: "CapabilityId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Capabilities_OntologyTerms_OntologyTermId",
                table: "Capabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Capabilities_Organisations_OrganisationId",
                table: "Capabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_Capabilities_SampleCollectionModes_SampleCollectionModeId",
                table: "Capabilities");

            migrationBuilder.DropForeignKey(
                name: "FK_CapabilityAssociatedDatas_Capabilities_DiagnosisCapabilityId",
                table: "CapabilityAssociatedDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Capabilities",
                table: "Capabilities");

            migrationBuilder.RenameTable(
                name: "Capabilities",
                newName: "DiagnosisCapabilities");

            migrationBuilder.RenameColumn(
                name: "CapabilityId",
                table: "DiagnosisCapabilities",
                newName: "DiagnosisCapabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Capabilities_SampleCollectionModeId",
                table: "DiagnosisCapabilities",
                newName: "IX_DiagnosisCapabilities_SampleCollectionModeId");

            migrationBuilder.RenameIndex(
                name: "IX_Capabilities_OrganisationId",
                table: "DiagnosisCapabilities",
                newName: "IX_DiagnosisCapabilities_OrganisationId");

            migrationBuilder.RenameIndex(
                name: "IX_Capabilities_OntologyTermId",
                table: "DiagnosisCapabilities",
                newName: "IX_DiagnosisCapabilities_OntologyTermId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DiagnosisCapabilities",
                table: "DiagnosisCapabilities",
                column: "DiagnosisCapabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_CapabilityAssociatedDatas_DiagnosisCapabilities_DiagnosisCapabilityId",
                table: "CapabilityAssociatedDatas",
                column: "DiagnosisCapabilityId",
                principalTable: "DiagnosisCapabilities",
                principalColumn: "DiagnosisCapabilityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisCapabilities_OntologyTerms_OntologyTermId",
                table: "DiagnosisCapabilities",
                column: "OntologyTermId",
                principalTable: "OntologyTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisCapabilities_Organisations_OrganisationId",
                table: "DiagnosisCapabilities",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "OrganisationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisCapabilities_SampleCollectionModes_SampleCollectionModeId",
                table: "DiagnosisCapabilities",
                column: "SampleCollectionModeId",
                principalTable: "SampleCollectionModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
