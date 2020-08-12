using Microsoft.EntityFrameworkCore;
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
        
        public PublicationDbContext() : base()
        { }

        public PublicationDbContext(DbContextOptions<PublicationDbContext> options) : base(options)
        { }

        public DbSet<Publication> Publications { get; set; }

       

    }
}

