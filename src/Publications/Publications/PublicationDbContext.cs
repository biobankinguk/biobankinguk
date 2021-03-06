using Directory.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Publications.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Publications
{
    public class PublicationDbContext : DbContext
    {
        public PublicationDbContext() : base() { }

        public PublicationDbContext(DbContextOptions<PublicationDbContext> options) : base(options) { }

        public DbSet<Publication> Publications { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Annotation> Annotations { get; set; }

        public DbSet<PublicationAnnotation> PublicationAnnotations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PublicationAnnotation>()
                .HasKey(bc => new { bc.Publication_Id, bc.Annotation_Id });
            modelBuilder.Entity<PublicationAnnotation>()
                .HasOne(bc => bc.Publication)
                .WithMany(b => b.PublicationAnnotations)
                .HasForeignKey(bc => bc.Publication_Id);
            modelBuilder.Entity<PublicationAnnotation>()
                .HasOne(bc => bc.Annotation)
                .WithMany(c => c.PublicationAnnotations)
                .HasForeignKey(bc => bc.Annotation_Id);
        }
    }
}
