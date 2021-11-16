using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLServer.GraphQL
{
    /// <summary>
    /// Запросы GraphQl (запросы выполняются параллельно)
    /// </summary>
    public class Queries
    {
        private ILogger<Queries> _logger;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger"></param>
        public Queries(ILogger<Queries> logger)
        {
            _logger = logger;

            logger.LogInformation("Инициализация Query");
        }

        /// <summary>
        /// Запрос чтения
        /// </summary>
        /// <param name="appDbContext">Контекст базы данных Entity</param>   
        /// <returns>Каталог</returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Catalog> Catalog([Service] AppDbContext appDbContext) => appDbContext.Catalogs;
        /*{
            return new List<Catalog>
            {
                new Catalog("Line") { Id = 1 },
                new Catalog("Reel") { Id = 2 }
            }
            .AsQueryable();
        }*/
    }
}
