using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using HotChocolate.Data;
using System.Collections.Generic;
using System.Linq;

namespace GraphQLServer.GraphQL
{
    public class Query
    {
        [UseProjection]
        public IQueryable<Catalog> Catalog([Service] AppDbContext appDbContext) => appDbContext.Catalogs;
        /*{
            return appDbContext.Catalogs;

            return new List<Catalog>
            {
                new Catalog("Line") { Id = 1 },
                new Catalog("Reel") { Id = 2 }
            }
            .AsQueryable();
        }*/
    }
}
