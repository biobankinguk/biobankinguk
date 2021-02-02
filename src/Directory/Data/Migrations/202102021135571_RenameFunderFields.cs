namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameFunderFields : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FunderOrganisations", "Funder_FunderId", "dbo.Funders");
            RenameColumn(table: "dbo.FunderOrganisations", name: "Funder_FunderId", newName: "Funder_Id");
            RenameIndex(table: "dbo.FunderOrganisations", name: "IX_Funder_FunderId", newName: "IX_Funder_Id");
            DropPrimaryKey("dbo.Funders");
            
            RenameColumn("dbo.Funders", "FunderId", "Id");
            RenameColumn("dbo.Funders", "Name", "Value");

            AddPrimaryKey("dbo.Funders", "Id");
            AddForeignKey("dbo.FunderOrganisations", "Funder_Id", "dbo.Funders", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FunderOrganisations", "Funder_Id", "dbo.Funders");
            DropPrimaryKey("dbo.Funders");

            RenameColumn("dbo.Funders", "Id", "FunderId");
            RenameColumn("dbo.Funders", "Value", "Name");

            AddPrimaryKey("dbo.Funders", "FunderId");
            RenameIndex(table: "dbo.FunderOrganisations", name: "IX_Funder_Id", newName: "IX_Funder_FunderId");
            RenameColumn(table: "dbo.FunderOrganisations", name: "Funder_Id", newName: "Funder_FunderId");
            AddForeignKey("dbo.FunderOrganisations", "Funder_FunderId", "dbo.Funders", "FunderId", cascadeDelete: true);
        }
    }
}
