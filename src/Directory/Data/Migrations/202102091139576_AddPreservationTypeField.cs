namespace Biobanks.Directory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPreservationTypeField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.LiveSamples", "PreservationTypeId", c => c.Int());
            AddColumn("dbo.StagedSamples", "PreservationTypeId", c => c.Int());
            CreateIndex("dbo.LiveSamples", "PreservationTypeId");
            CreateIndex("dbo.StagedSamples", "PreservationTypeId");
            AddForeignKey("dbo.LiveSamples", "PreservationTypeId", "dbo.PreservationTypes", "Id");
            AddForeignKey("dbo.StagedSamples", "PreservationTypeId", "dbo.PreservationTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StagedSamples", "PreservationTypeId", "dbo.PreservationTypes");
            DropForeignKey("dbo.LiveSamples", "PreservationTypeId", "dbo.PreservationTypes");
            DropIndex("dbo.StagedSamples", new[] { "PreservationTypeId" });
            DropIndex("dbo.LiveSamples", new[] { "PreservationTypeId" });
            DropColumn("dbo.StagedSamples", "PreservationTypeId");
            DropColumn("dbo.LiveSamples", "PreservationTypeId");
        }
    }
}
