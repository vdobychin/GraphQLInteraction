using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLServer.GraphQL
{
    public class Query
    {
        private ILogger<Query> _logger;
        public Query(ILogger<Query> logger)
        {
            _logger = logger;

            logger.LogInformation("Инициализация");
        }


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
