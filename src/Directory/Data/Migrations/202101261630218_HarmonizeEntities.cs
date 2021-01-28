namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HarmonizeEntities : DbMigration
    {
        public override void Up()
        {
            HarmonizeSexUp();
            HarmonizePreservationTypeUp();
            HarmonizeMaterialTypeUp();
            HarmonizeDiagnosisUp();
        }

        public override void Down()
        {
            HarmonizeSexDown();
            HarmonizePreservationTypeDown();
            HarmonizeMaterialTypeDown();
            HarmonizeDiagnosisDown();
        }

        private void HarmonizeSexUp()
        {
            RenameColumn("dbo.Sexes", "SexId", "Id");
            RenameColumn("dbo.Sexes", "Description", "Value");
        }

        private void HarmonizeSexDown()
        {
            RenameColumn("dbo.Sexes", "Id", "SexId");
            RenameColumn("dbo.Sexes", "Value", "Description");
        }

        private void HarmonizePreservationTypeUp()
        {
            // PreservationTypes => StorageTemperature
            RenameColumn("dbo.PreservationTypes", "PreservationTypeId", "Id");
            RenameColumn("dbo.PreservationTypes", "Description", "Value");
            RenameTable("dbo.PreservationTypes", "StorageTemperatures");

            // MaterialDetails (Drop, Rename, Recreate) Index and Foreign/Primary Keys
            DropForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes");
            DropIndex("dbo.MaterialDetails", new[] { "PreservationTypeId" });
            DropPrimaryKey("dbo.MaterialDetails");

            RenameColumn("dbo.MaterialDetails", "PreservationTypeId", "StorageTemperatureId");

            AddPrimaryKey("dbo.MaterialDetails", new[] { "SampleSetId", "MaterialTypeId", "StorageTemperatureId", "MacroscopicAssessmentId" });
            CreateIndex("dbo.MaterialDetails", "StorageTemperatureId");
            AddForeignKey("dbo.MaterialDetails", "StorageTemperatureId", "dbo.StorageTemperatures", "Id", cascadeDelete: true);
        }

        private void HarmonizePreservationTypeDown()
        {
            // StorageTemperature => PreservationTypes
            RenameColumn("dbo.StorageTemperatures", "Id", "PreservationTypeId");
            RenameColumn("dbo.StorageTemperatures", "Value", "Description");
            RenameTable("dbo.StorageTemperatures", "PreservationTypes");

            // MaterialDetails (Drop, Rename, Recreate) Index and Foreign/Primary Keys
            DropForeignKey("dbo.MaterialDetails", "StorageTemperatureId", "dbo.StorageTemperatures");
            DropIndex("dbo.MaterialDetails", new[] { "StorageTemperatureId" });
            DropPrimaryKey("dbo.MaterialDetails");

            RenameColumn("dbo.MaterialDetails", "StorageTemperatureId", "PreservationTypeId");

            AddPrimaryKey("dbo.MaterialDetails", new[] { "SampleSetId", "MaterialTypeId", "PreservationTypeId", "MacroscopicAssessmentId" });
            CreateIndex("dbo.MaterialDetails", "PreservationTypeId");
            AddForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes", "PreservationTypeId", cascadeDelete: true);
        }

        private void HarmonizeMaterialTypeUp()
        {
            DropForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes");
            DropPrimaryKey("dbo.MaterialTypes");

            CreateTable(
                "dbo.MaterialTypeGroups",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.MaterialTypeGroupMaterialTypes",
                c => new
                {
                    MaterialTypeGroupId = c.Int(nullable: false),
                    MaterialTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.MaterialTypeGroupId, t.MaterialTypeId })
                .ForeignKey("dbo.MaterialTypeGroups", t => t.MaterialTypeGroupId, cascadeDelete: true)
                .ForeignKey("dbo.MaterialTypes", t => t.MaterialTypeId, cascadeDelete: true)
                .Index(t => t.MaterialTypeGroupId)
                .Index(t => t.MaterialTypeId);

            RenameColumn("dbo.MaterialTypes", "MaterialTypeId", "Id");
            RenameColumn("dbo.MaterialTypes", "Description", "Value");

            AddPrimaryKey("dbo.MaterialTypes", "Id");
            AddForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes", "Id", cascadeDelete: true);
        }

        private void HarmonizeMaterialTypeDown()
        {
            DropForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroupId", "dbo.MaterialTypeGroups");
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeId" });
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeGroupId" });
            DropPrimaryKey("dbo.MaterialTypes");

            RenameColumn("dbo.MaterialTypes", "Id", "MaterialTypeId");
            RenameColumn("dbo.MaterialTypes", "Value", "Description");

            DropTable("dbo.MaterialTypeGroupMaterialTypes");
            DropTable("dbo.MaterialTypeGroups");
            AddPrimaryKey("dbo.MaterialTypes", "MaterialTypeId");
            AddForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes", "MaterialTypeId", cascadeDelete: true);
        }

        private void HarmonizeDiagnosisUp()
        {
            // New Foreign Keys - Created First So The Keys Can Be Populated For Exisiting Records
            AddColumn("dbo.Collections", "SnomedTermId", c => c.String(maxLength: 20));
            AddColumn("dbo.DiagnosisCapabilities", "SnomedTermId", c => c.String(maxLength: 20));

            // Copy Across What Will Become New Foreign Keys (Get SnomedIdentifier For Given DiagnosisId)
            Sql("UPDATE dbo.Collections SET SnomedTermId = d.SnomedIdentifier FROM dbo.Diagnosis d WHERE dbo.Collections.DiagnosisId = d.DiagnosisId");
            Sql("UPDATE dbo.DiagnosisCapabilities SET SnomedTermId = d.SnomedIdentifier FROM dbo.Diagnosis d WHERE dbo.DiagnosisCapabilities.DiagnosisId = d.DiagnosisId");

            DropForeignKey("dbo.Collections", "DiagnosisId", "dbo.Diagnosis");
            DropForeignKey("dbo.DiagnosisCapabilities", "DiagnosisId", "dbo.Diagnosis");
            DropIndex("dbo.Collections", new[] { "DiagnosisId" });
            DropIndex("dbo.Diagnosis", new[] { "Description" });
            DropIndex("dbo.DiagnosisCapabilities", new[] { "DiagnosisId" });
            DropPrimaryKey("dbo.Diagnosis");

            // Remove Duplicate Records (Allowing For New PK Constraint Later)
            Sql("DELETE FROM Diagnosis WHERE Diagnosis.SnomedIdentifier = '49049000' AND Diagnosis.Description = 'Parkinson''s disease (disorder)'");
            Sql("DELETE FROM Diagnosis WHERE Diagnosis.SnomedIdentifier = '66071002' AND Diagnosis.Description = 'Viral hepatitis type B (disorder)'");

            CreateTable(
                "dbo.SnomedTags",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            RenameTable("dbo.Diagnosis", "SnomedTerms");

            RenameColumn("dbo.SnomedTerms", "SnomedIdentifier", "Id");
            AlterColumn("dbo.SnomedTerms", "Id", c => c.String(maxLength: 20, nullable: false));
            AddColumn("dbo.SnomedTerms", "SnomedTagId", c => c.Int(nullable: true));

            AddPrimaryKey("dbo.SnomedTerms", "Id");
            CreateIndex("dbo.SnomedTerms", new[] { "SnomedTagId" });
            CreateIndex("dbo.SnomedTerms", new[] { "Description" });
            CreateIndex("dbo.Collections", "SnomedTermId");
            CreateIndex("dbo.DiagnosisCapabilities", "SnomedTermId");
            AddForeignKey("dbo.SnomedTerms", "SnomedTagId", "dbo.SnomedTags");
            AddForeignKey("dbo.Collections", "SnomedTermId", "dbo.SnomedTerms");
            AddForeignKey("dbo.DiagnosisCapabilities", "SnomedTermId", "dbo.SnomedTerms", "Id");

            DropColumn("dbo.SnomedTerms", "DiagnosisId");
            DropColumn("dbo.Collections", "DiagnosisId");
            DropColumn("dbo.DiagnosisCapabilities", "DiagnosisId");
        }

        private void HarmonizeDiagnosisDown()
        {
            AddColumn("dbo.DiagnosisCapabilities", "DiagnosisId", c => c.Int(nullable: false));
            AddColumn("dbo.Collections", "DiagnosisId", c => c.Int(nullable: false));
            AddColumn("dbo.SnomedTerms", "DiagnosisId", c => c.Int(nullable: false, identity: true));

            // Copy Across What Will Become Old Foreign Keys (Get Generated DiagnosisId For Given SnomedIdentifier)
            Sql("UPDATE dbo.Collections SET DiagnosisId = d.DiagnosisId FROM dbo.SnomedTerms d WHERE dbo.Collections.SnomedTermId = d.Id");
            Sql("UPDATE dbo.DiagnosisCapabilities SET DiagnosisId = d.DiagnosisId FROM dbo.SnomedTerms d WHERE dbo.DiagnosisCapabilities.SnomedTermId = d.Id");

            DropForeignKey("dbo.DiagnosisCapabilities", "SnomedTermId", "dbo.SnomedTerms");
            DropForeignKey("dbo.Collections", "SnomedTermId", "dbo.SnomedTerms");
            DropForeignKey("dbo.SnomedTerms", "SnomedTagId", "dbo.SnomedTags");
            DropIndex("dbo.DiagnosisCapabilities", new[] { "SnomedTermId" });
            DropIndex("dbo.Collections", new[] { "SnomedTermId" });
            DropIndex("dbo.SnomedTerms", new[] { "SnomedTagId" });
            DropIndex("dbo.SnomedTerms", new[] { "Description" });
            DropPrimaryKey("dbo.SnomedTerms");
            DropTable("dbo.SnomedTags");

            RenameTable("dbo.SnomedTerms", "Diagnosis");

            RenameColumn("dbo.Diagnosis", "Id", "SnomedIdentifier");
            AlterColumn("dbo.Diagnosis", "SnomedIdentifier", c => c.String(maxLength: 20, nullable: true));

            AddPrimaryKey("dbo.Diagnosis", "DiagnosisId");
            CreateIndex("dbo.DiagnosisCapabilities", "DiagnosisId");
            CreateIndex("dbo.Diagnosis", "Description", unique: true);
            CreateIndex("dbo.Collections", "DiagnosisId");
            AddForeignKey("dbo.DiagnosisCapabilities", "DiagnosisId", "dbo.Diagnosis", "DiagnosisId", cascadeDelete: true);
            AddForeignKey("dbo.Collections", "DiagnosisId", "dbo.Diagnosis", "DiagnosisId", cascadeDelete: true);

            DropColumn("dbo.DiagnosisCapabilities", "SnomedTermId");
            DropColumn("dbo.Collections", "SnomedTermId");
            DropColumn("dbo.Diagnosis", "SnomedTagId");
        }
    }
}
