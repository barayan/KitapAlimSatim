using KitapAlimSatim.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KitapAlimSatim.Data
{
    public class KitapAlimSatimDbContext : DbContext
    {
        public KitapAlimSatimDbContext(DbContextOptions<KitapAlimSatimDbContext> options) : base(options) { }

        // Entites
        public DbSet<Book> Book { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(KitapAlimSatimDbContext).Assembly);
        }
    }
}
