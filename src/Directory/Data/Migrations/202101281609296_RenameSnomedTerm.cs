namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSnomedTerm : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SnomedTerms", newName: "OntologyTerms");
            RenameColumn(table: "dbo.Collections", name: "SnomedTermId", newName: "OntologyTermId");
            RenameColumn(table: "dbo.DiagnosisCapabilities", name: "SnomedTermId", newName: "OntologyTermId");
            RenameIndex(table: "dbo.Collections", name: "IX_SnomedTermId", newName: "IX_OntologyTermId");
            RenameIndex(table: "dbo.DiagnosisCapabilities", name: "IX_SnomedTermId", newName: "IX_OntologyTermId");
        }

        public override void Down()
        {
            RenameIndex(table: "dbo.DiagnosisCapabilities", name: "IX_OntologyTermId", newName: "IX_SnomedTermId");
            RenameIndex(table: "dbo.Collections", name: "IX_OntologyTermId", newName: "IX_SnomedTermId");
            RenameColumn(table: "dbo.DiagnosisCapabilities", name: "OntologyTermId", newName: "SnomedTermId");
            RenameColumn(table: "dbo.Collections", name: "OntologyTermId", newName: "SnomedTermId");
            RenameTable(name: "dbo.OntologyTerms", newName: "SnomedTerms");
        }
    }
}
