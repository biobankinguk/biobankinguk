namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HarmonizeSex : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Sexes", "SexId", "Id");
            RenameColumn("dbo.Sexes", "Description", "Value");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Sexes", "Id", "SexId");
            RenameColumn("dbo.Sexes", "Value", "Description");
        }
    }
}
