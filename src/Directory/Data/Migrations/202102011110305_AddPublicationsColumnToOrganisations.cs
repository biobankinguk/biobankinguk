namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPublicationsColumnToOrganisations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Organisations", "IncludePublications", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Organisations", "IncludePublications");
        }
    }
}
