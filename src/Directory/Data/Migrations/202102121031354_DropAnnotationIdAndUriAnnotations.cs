namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropAnnotationIdAndUriAnnotations : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Annotations", "AnnotationId");
            DropColumn("dbo.Annotations", "Uri");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Annotations", "Uri", c => c.String());
            AddColumn("dbo.Annotations", "AnnotationId", c => c.String());
        }
    }
}
