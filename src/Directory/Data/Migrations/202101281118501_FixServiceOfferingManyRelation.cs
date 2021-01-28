namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixServiceOfferingManyRelation : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.OrganisationServiceOfferings");
            RenameColumn("dbo.OrganisationServiceOfferings", "ServiceId", "ServiceOfferingId");
            AddPrimaryKey("dbo.OrganisationServiceOfferings", new[] { "OrganisationId", "ServiceOfferingId" });
            CreateIndex("dbo.OrganisationServiceOfferings", "ServiceOfferingId");
            AddForeignKey("dbo.OrganisationServiceOfferings", "ServiceOfferingId", "dbo.ServiceOfferings", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrganisationServiceOfferings", "ServiceOfferingId", "dbo.ServiceOfferings");
            DropIndex("dbo.OrganisationServiceOfferings", new[] { "ServiceOfferingId" });
            DropPrimaryKey("dbo.OrganisationServiceOfferings");
            RenameColumn("dbo.OrganisationServiceOfferings", "ServiceOfferingId", "ServiceId");
            AddPrimaryKey("dbo.OrganisationServiceOfferings", new[] { "OrganisationId", "ServiceId" });
        }
    }
}
