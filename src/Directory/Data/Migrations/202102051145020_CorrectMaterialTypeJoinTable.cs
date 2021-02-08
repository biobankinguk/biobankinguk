using System.Data.Entity.Migrations;

namespace Biobanks.Directory.Data.Migrations
{
    public partial class CorrectMaterialTypeJoinTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroupId", "dbo.MaterialTypeGroups");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeId", "dbo.MaterialTypes");
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeGroupId" });
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeId" });
            DropPrimaryKey("dbo.MaterialTypeGroupMaterialTypes");

            RenameColumn("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroupId", "MaterialTypeGroup_Id");
            RenameColumn("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeId", "MaterialType_Id");

            AddPrimaryKey("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialType_Id", "MaterialTypeGroup_Id" });
            CreateIndex("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroup_Id");
            CreateIndex("dbo.MaterialTypeGroupMaterialTypes", "MaterialType_Id");
            AddForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroup_Id", "dbo.MaterialTypeGroups");
            AddForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialType_Id", "dbo.MaterialTypes");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroup_Id", "dbo.MaterialTypeGroups");
            DropForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialType_Id", "dbo.MaterialTypes");
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new [] { "MaterialTypeGroup_Id" });
            DropIndex("dbo.MaterialTypeGroupMaterialTypes", new [] { "MaterialType_Id" });
            DropPrimaryKey("dbo.MaterialTypeGroupMaterialTypes");

            RenameColumn("dbo.MaterialTypeGroupMaterialTypes", "MaterialType_Id", "MaterialTypeId");
            RenameColumn("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroup_Id", "MaterialTypeGroupId");
            
            AddPrimaryKey("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeId", "MaterialTypeGroupId" });
            CreateIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeId" });
            CreateIndex("dbo.MaterialTypeGroupMaterialTypes", new[] { "MaterialTypeGroupId" });
            AddForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeId", "dbo.MaterialTypes");
            AddForeignKey("dbo.MaterialTypeGroupMaterialTypes", "MaterialTypeGroupId", "dbo.MaterialTypeGroups");
        }
    }
}
