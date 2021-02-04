namespace Biobanks.Directory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnotationEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Annotations",
                    c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnnotationId = c.String(),
                        Name = c.String(),
                        Uri = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                    "dbo.PublicationAnnotations",
                    c => new
                    {
                        Publication_Id = c.Int(nullable: false),
                        Annotation_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Publication_Id, t.Annotation_Id })
                .ForeignKey("dbo.Publications", t => t.Publication_Id, cascadeDelete: true)
                .ForeignKey("dbo.Annotations", t => t.Annotation_Id, cascadeDelete: true)
                .Index(t => t.Publication_Id)
                .Index(t => t.Annotation_Id);

            AddColumn("dbo.Publications", "Source", c => c.String());
        }

        public override void Down()
        {
            DropForeignKey("dbo.PublicationAnnotations", "Annotation_Id", "dbo.Annotations");
            DropForeignKey("dbo.PublicationAnnotations", "Publication_Id", "dbo.Publications");
            DropIndex("dbo.PublicationAnnotations", new[] { "Annotation_Id" });
            DropIndex("dbo.PublicationAnnotations", new[] { "Publication_Id" });
            DropColumn("dbo.Publications", "Source");
            DropTable("dbo.PublicationAnnotations");
            DropTable("dbo.Annotations");
        }
    }
}
