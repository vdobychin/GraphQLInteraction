using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
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
        /// Запрос чтения каталога
        /// </summary>
        /// <param name="appDbContext">Контекст базы данных Entity</param>   
        /// <returns>Каталог</returns>
        //[UsePaging(IncludeTotalCount = true)] //Для постраничного вывода
        //[UseProjection]
        //[UseFiltering]
        //[UseSorting]
        [Authorize(Roles = new[] { "admin" })]
        public IQueryable<Catalog> Catalog([Service] AppDbContext appDbContext) => appDbContext.Catalogs;

        /// <summary>
        /// Запрос чтения подкаталога
        /// </summary>
        /// <param name="appDbContext"></param>
        /// <returns>Подкаталог</returns>
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<SubCatalog> SubCatalog([Service] AppDbContext appDbContext) => appDbContext.SubCatalogs;

    }
}
