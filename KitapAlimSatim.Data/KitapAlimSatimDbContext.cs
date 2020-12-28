using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data
{
    public class KitapAlimSatimDbContext : DbContext
    {
        public KitapAlimSatimDbContext(DbContextOptions<KitapAlimSatimDbContext> options) : base(options)
        {

        }

        public KitapAlimSatimDbContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KitapAlimSatimDbContext).Assembly);
        }
    }
}
