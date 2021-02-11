namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MaterialTypeJoinRefresh : DbMigration
    {
        public override void Up()
        {
            // Intentionally blank - it was required at the time to refresh the DbContext when
            // the MaterialTypeGroupMaterialType join was explicitly defined with the Fluent API.
        }
        
        public override void Down()
        {
        }
    }
}
