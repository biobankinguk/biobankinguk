using Microsoft.EntityFrameworkCore.Migrations;

namespace Biobanks.Data.Migrations
{
    public partial class FixPublicationAnnotations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //Drop PublicationAnnotations and Add AnnotationPublication
            migrationBuilder.Sql("DROP TABLE IF EXISTS dbo.PublicationAnnotations");
            
            migrationBuilder.Sql("IF OBJECT_ID('dbo.AnnotationPublication', 'U') IS NULL " +
                "CREATE TABLE [dbo].[AnnotationPublication] (" +
                "[AnnotationsId]  INT NOT NULL," +
                "[PublicationsId] INT NOT NULL," +
                "CONSTRAINT[PK_AnnotationPublication] PRIMARY KEY CLUSTERED([AnnotationsId] ASC, [PublicationsId] ASC)," +
                "CONSTRAINT[FK_AnnotationPublication_Annotations_AnnotationsId] FOREIGN KEY([AnnotationsId]) REFERENCES[dbo].[Annotations]([Id]) ON DELETE CASCADE," +
                "CONSTRAINT[FK_AnnotationPublication_Publications_PublicationsId] FOREIGN KEY([PublicationsId]) REFERENCES[dbo].[Publications]([Id]) ON DELETE CASCADE);");
            
            migrationBuilder.Sql("IF INDEXPROPERTY(OBJECT_ID('dbo.AnnotationPublication'), 'IX_AnnotationPublication_PublicationsId', 'IndexID') IS NULL " +
               "CREATE NONCLUSTERED INDEX[IX_AnnotationPublication_PublicationsId]" +
                "ON[dbo].[AnnotationPublication]([PublicationsId] ASC);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
