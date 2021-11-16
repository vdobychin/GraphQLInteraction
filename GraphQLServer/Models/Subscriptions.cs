using HotChocolate;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Models
{
    /// <summary>
    /// Подписки GraphQl
    /// </summary>
    public class Subscriptions
    {
        /// <summary>
        /// Каталог изменился
        /// </summary>
        /// <param name="catalog"></param>
        /// <returns>Каталог</returns>
        [Subscribe]
        public Catalog OnCatalogChange([EventMessage] Catalog catalog) => catalog;
    }
}
