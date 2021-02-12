namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPreservationFieldMaterialDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MaterialDetails", "PreservationTypeId", c => c.Int());
            CreateIndex("dbo.MaterialDetails", "PreservationTypeId");
            AddForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaterialDetails", "PreservationTypeId", "dbo.PreservationTypes");
            DropIndex("dbo.MaterialDetails", new[] { "PreservationTypeId" });
            DropColumn("dbo.MaterialDetails", "PreservationTypeId");
        }
    }
}
