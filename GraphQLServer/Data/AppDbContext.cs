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

                // Один ко многим
                b.HasMany(a => a.SubCatalogs)
                 .WithOne(b => b.Catalog);
            });

            modelBuilder.Entity<SubCatalog>(b =>
            {
                var subcatalog = new SubCatalog[]
                {
                    new() { Id = 1, Name = "Плетеные шнуры", CatalogId = 1 },
                    new() { Id = 2, Name = "Монофильная леска", CatalogId = 1 },
                    new() { Id = 3, Name = "Флюорокарбоновая леска", CatalogId = 1 },
                    new() { Id = 4, Name = "Безынерционные катушки", CatalogId = 2 },
                    new() { Id = 5, Name = "Инерционные катушки", CatalogId = 2 },
                    new() { Id = 6, Name = "Катушки с байтраннером", CatalogId = 2 },
                    new() { Id = 7, Name = "Матчевые катушки", CatalogId = 2 },
                    new() { Id = 8, Name = "Фидерные катушки", CatalogId = 2 },
                    new() { Id = 9, Name = "Шпули для катушек", CatalogId = 2 },
                    new() { Id = 10, Name = "Одинарные крючки", CatalogId = 3 },
                    new() { Id = 11, Name = "Двойные крючки", CatalogId = 3 },
                    new() { Id = 12, Name = "Тройные крючки", CatalogId = 3 },
                    new() { Id = 13, Name = "Офсетные крючки", CatalogId = 3 },
                    new() { Id = 14, Name = "Крючки незацепляйки", CatalogId = 3 },
                    new() { Id = 15, Name = "Крючки с поводком", CatalogId = 3 }
                };

                b.HasKey(a => a.Id);
                b.HasIndex(b => b.Name).IsUnique();
                b.HasData(subcatalog);
            });

        }
    }
}
