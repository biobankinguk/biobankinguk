namespace Biobanks.Directory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddPreservationType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PreservationTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        StorageTemperatureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StorageTemperatures", t => t.StorageTemperatureId, cascadeDelete: true)
                .Index(t => t.StorageTemperatureId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PreservationTypes", "StorageTemperatureId", "dbo.StorageTemperatures");
            DropIndex("dbo.PreservationTypes", new[] { "StorageTemperatureId" });
            DropTable("dbo.PreservationTypes");
        }
    }
}
