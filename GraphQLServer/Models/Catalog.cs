using HotChocolate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Models
{
    /// <summary>
    /// Каталог
    /// </summary>
    [GraphQLDescription("Каталог")]
    public class Catalog
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public int Id { get; set; } //Уникальный идентификатор

        /// <summary>
        /// Имя каталога
        /// </summary>
        [Required]
        public string Name { get; set; }

        /** Подкаталоги каталога */
        //[Authorize(Roles = new[] { "admin" })]
        public ICollection<SubCatalog> SubCatalogs { get; set; } = new List<SubCatalog>();
    }
}
