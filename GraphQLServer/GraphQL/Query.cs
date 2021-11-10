using GraphQLServer.Data;
using GraphQLServer.Models;
using HotChocolate;
using System.Linq;

namespace GraphQLServer.GraphQL
{
    public class Query
    {
        public IQueryable<Catalog> GetCatalog([Service] AppDbContext context)
        {
            return context.Catalogs;
        }
    }
}
