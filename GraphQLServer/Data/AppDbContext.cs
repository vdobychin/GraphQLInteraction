using GraphQLServer.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;

namespace GraphQLServer.Data
{
    /** */
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Констрктор DemoContext
        /// </summary>
        /// <param name="options">свойства контекста</param>
        public AppDbContext(DbContextOptions options) : base(options)
        {
            try
            {
                Database.EnsureDeleted();   // удаляем бд со старой схемой
                Database.EnsureCreated();   // создаем бд с новой схемой
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
        }

        /** Каталог */
        public DbSet<Catalog> Catalogs { get; set; }

        /** Подкаталог */
        public DbSet<SubCatalog> SubCatalogs { get; set; }

        /** Переопределение метода */
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = System.IO.Path.Combine(AppContext.BaseDirectory, "gc.db") }.ToString());
            optionsBuilder.UseSqlite(connection);
        }

        /** Создание модели */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("catalogs");

            var catalogs = new Catalog[]
            {
                new() { Id = 1, Name = "Line" },
                new() { Id = 2, Name = "Reel" },
                new() { Id = 3, Name = "Hook" },
                new() { Id = 4, Name = "Feeder" }
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
