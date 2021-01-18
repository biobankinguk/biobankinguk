namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HarmonizePreservationType : DbMigration
    {
        public override void Up()
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
        
        public override void Down()
        {
            // StorageTemperature => PreservationTypes
            RenameColumn("dbo.StorageTemperatures", "Id", "PreservationTypeId");
            RenameColumn("dbo.StorageTemperatures", "Value", "Description");
            RenameTable("dbo.StorageTemperatures", "PreservationTypes");

            // MaterialDetails (Drop, Rename, Recreate) Index and Foreign/Primary Keys
            DropForeignKey("dbo.MaterialDetails", "StorageTemperatureId", "dbo.StorageTemperatures");
            DropIndex("dbo.MaterialDetails", new[] { "StorageTemperatureId" });
            DropPrimaryKey("dbo.MaterialDetails");

            RenameColumn("dbo.MaterialDetails","StorageTemperatureId", "PreservationTypeId");

            AddPrimaryKey("dbo.MaterialDetails", new[] { "SampleSetId", "MaterialTypeId", "PreservationTypeId", "MacroscopicAssessmentId" });
            CreateIndex("dbo.MaterialDetails", "PreservationTypeId");
            AddForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes", "PreservationTypeId", cascadeDelete: true);
        }
    }
}
