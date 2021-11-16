using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.GraphQL
{
    /// <summary>
    /// Мутации GraphQl (запросы выполняются последовательно (транзакция))
    /// </summary>
    public class Mutations
    {
        private ILogger<Queries> _logger;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="logger"></param>
        public Mutations(ILogger<Queries> logger)
        {
            _logger = logger;
            logger.LogInformation("Инициализация Mutations");
        }

        # region CreateCatalog
        /// <summary>
        /// Создание каталога
        /// </summary>
        /// <param name="catalog">Создаваемый каталог</param>
        /// <param name="appDbContext">Контекст базы данных Entity</param>
        /// <param name="sender"></param>
        /// <returns>Каталог</returns>
        public async Task<Catalog> CreateCatalog(Catalog catalog, [Service] AppDbContext appDbContext, [Service] ITopicEventSender sender)
        {
            appDbContext.Add(catalog);
            appDbContext.SaveChanges();
            await sender.SendAsync(nameof(Subscriptions.OnCatalogChange), catalog);
            return catalog;
        }
        #endregion

        # region DeleteCatalog
        /// <summary>
        /// Удаление каталога
        /// </summary>
        /// <param name="catalog">Создаваемый каталог</param>
        /// <param name="appDbContext">Контекст базы данных Entity</param>
        /// <returns>Каталог</returns>
        public Catalog DeleteCatalog(Catalog catalog, [Service] AppDbContext appDbContext)
        {
            appDbContext.Remove(catalog);
            appDbContext.SaveChanges();
            return catalog;
        }
        #endregion
    }
}
