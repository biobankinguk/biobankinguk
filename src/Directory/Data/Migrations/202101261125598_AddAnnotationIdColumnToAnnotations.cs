namespace Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnotationIdColumnToAnnotations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Annotations", "AnnotationId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Annotations", "AnnotationId");
        }
    }
}
