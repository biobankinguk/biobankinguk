namespace Biobanks.Directory.Data.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class movedDiagnosisUniqueIndexToOnModelCreating : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Diagnosis", "IX_UniqueDiagnosisDescription");
            AlterColumn("dbo.Diagnosis", "Description", c => c.String(maxLength: 200));
            CreateIndex("dbo.Diagnosis", "Description", unique: true);
        }

        public override void Down()
        {
            DropIndex("dbo.Diagnosis", new[] { "Description" });
            AlterColumn("dbo.Diagnosis", "Description", c => c.String(nullable: false, maxLength: 200));
            CreateIndex("dbo.Diagnosis", "Description", unique: true, name: "IX_UniqueDiagnosisDescription");
        }
    }
}
