namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFromAPItoCollection : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Collections", "FromApi", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Collections", "FromApi");
        }
    }
}
