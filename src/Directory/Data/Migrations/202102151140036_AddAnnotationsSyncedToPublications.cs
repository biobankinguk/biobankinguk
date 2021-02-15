namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnotationsSyncedToPublications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Publications", "AnnotationsSynced", c => c.DateTime());
            DropColumn("dbo.Annotations", "AnnotationId");
            DropColumn("dbo.Annotations", "Uri");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Annotations", "Uri", c => c.String());
            AddColumn("dbo.Annotations", "AnnotationId", c => c.String());
            DropColumn("dbo.Publications", "AnnotationsSynced");
        }
    }
}
