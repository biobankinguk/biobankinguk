namespace Biobanks.Directory.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HarmonizeMaterialType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes");
            DropPrimaryKey("dbo.MaterialTypes");

            CreateTable(
                "dbo.MaterialTypeGroups",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Value = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.MaterialTypeGroupMaterialTypes",
                c => new
                {
                    MaterialTypeGroupId = c.Int(nullable: false),
                    MaterialTypeId = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.MaterialTypeGroupId, t.MaterialTypeId })
                .ForeignKey("dbo.MaterialTypeGroups", t => t.MaterialTypeGroupId, cascadeDelete: true)
                .ForeignKey("dbo.MaterialTypes", t => t.MaterialTypeId, cascadeDelete: true)
                .Index(t => t.MaterialTypeGroupId)
                .Index(t => t.MaterialTypeId);

            RenameColumn("dbo.MaterialTypes", "MaterialTypeId", "Id");
            RenameColumn("dbo.MaterialTypes", "Description", "Value");

            AddPrimaryKey("dbo.MaterialTypes", "Id");
            AddForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialType_Id", "dbo.MaterialTypes");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroup_Id", "dbo.MaterialTypeGroups");
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialType_Id" });
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeGroup_Id" });
            DropPrimaryKey("dbo.MaterialTypes");

            RenameColumn("dbo.MaterialTypes","Id", "MaterialTypeId");
            RenameColumn("dbo.MaterialTypes", "Value", "Description");

            DropTable("dbo.MaterialTypeGroupMaterialTypes");
            DropTable("dbo.MaterialTypeGroups");
            AddPrimaryKey("dbo.MaterialTypes", "MaterialTypeId");
            AddForeignKey("dbo.MaterialDetails", "MaterialTypeId", "dbo.MaterialTypes", "MaterialTypeId", cascadeDelete: true);
        }
    }
}
