namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameStatusTablePlural : DbMigration
    {
        public override void Up()
        {
            RenameTable("dbo.Status", "Statuses");
        }
        
        public override void Down()
        {
            RenameTable("dbo.Statuses", "Status");
        }
    }
}
