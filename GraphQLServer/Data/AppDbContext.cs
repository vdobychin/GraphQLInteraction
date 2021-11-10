using GraphQLServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<SubCatalog> SubCatalogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Catalogs
            modelBuilder.Entity<Catalog>().HasData(new Catalog("Line") { Id = 1 });
            modelBuilder.Entity<Catalog>().HasData(new Catalog("Reel") { Id = 2 });

            //SubCatalogs
            modelBuilder.Entity<SubCatalog>().HasData(new SubCatalog("Плетенка") { Id = 1 });
            base.OnModelCreating(modelBuilder);
        }

    }
}
