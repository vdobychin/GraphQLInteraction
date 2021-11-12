using GraphQLServer.Models;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.HasDefaultSchema("catalogs");
            var catalogs = new Catalog[]
            {
                new() {Id = 1, Name = "Line"},
                new() {Id = 2, Name = "Reel"}
            };

            modelBuilder.Entity<Catalog>(b =>
            {
                b.HasKey(a => a.Id);
                b.HasIndex(b => b.Name).IsUnique();
                b.HasData(catalogs);
            });
        }
    }
}
