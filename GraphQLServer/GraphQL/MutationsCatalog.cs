using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.GraphQL
{
    public class MutationsCatalog
    {
        private ILogger<Query> _logger;
        public MutationsCatalog(ILogger<Query> logger)
        {
            _logger = logger;

            logger.LogInformation("Инициализация Mutations");
        }


        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public Catalog CreateCatalog(Catalog catalog, [Service] AppDbContext appDbContext)
        {
            appDbContext.Add(catalog);
            appDbContext.SaveChanges();
            return catalog;
        }
    }
}
