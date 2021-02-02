namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnalytics : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DirectoryAnalyticEvents",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Date = c.DateTimeOffset(nullable: false),
                    EventCategory = c.String(nullable: true),
                    EventAction = c.String(nullable: true),
                    Biobank = c.String(nullable: true),
                    Segment = c.String(nullable: true),
                    Source = c.String(nullable: true),
                    Hostname = c.String(nullable: true),
                    City = c.String(nullable: true),
                    Counts = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.Id);


            CreateTable(
                "dbo.DirectoryAnalyticMetrics",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Date = c.DateTimeOffset(nullable: false),
                    PagePath = c.String(nullable: true),
                    PagePathLevel1 = c.String(nullable: true),
                    Segment = c.String(nullable: true),
                    Source = c.String(nullable: true),
                    Hostname = c.String(nullable: true),
                    City = c.String(nullable: true),
                    Sessions = c.Int(nullable: false),
                    BounceRate = c.Int(nullable: false),
                    PercentNewSessions = c.Int(nullable: false),
                    AvgSessionDuration = c.Int(nullable: false)
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.OrganisationAnalytics",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Date = c.DateTimeOffset(nullable: false),
                    PagePath = c.String(nullable: true),
                    PreviousPagePath = c.String(nullable: true),
                    Segment = c.String(nullable: true),
                    Source = c.String(nullable: true),
                    Hostname = c.String(nullable: true),
                    City = c.String(nullable: true),
                    Counts = c.Int(nullable: false),
                    OrganisationExternalId = c.String(nullable: true)
                })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.DirectoryAnalyticEvents");
            DropTable("dbo.DirectoryAnalyticMetrics");
            DropTable("dbo.OrganisationAnalytics");
        }
    }
}
