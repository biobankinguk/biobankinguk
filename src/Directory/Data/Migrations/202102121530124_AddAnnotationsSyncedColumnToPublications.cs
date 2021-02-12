namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnotationsSyncedColumnToPublications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Publications", "AnnotationsSynced", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Publications", "AnnotationsSynced");
        }
    }
}
