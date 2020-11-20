namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApprovedDateColumnOrganisationNetworks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrganisationNetworks", "ApprovedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrganisationNetworks", "ApprovedDate");
        }
    }
}
