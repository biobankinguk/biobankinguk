namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSourceColumnToPublications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Publications", "Source", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Publications", "Source");
        }
    }
}
