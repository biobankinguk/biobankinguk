namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaterialTypeRefresh : DbMigration
    {
        public override void Up()
        {
            // Intentionally blank, required to 'refresh' the MigrationHistory such that the
            // changes for the join table are recognised.
        }

        public override void Down()
        {
        }
    }
}
