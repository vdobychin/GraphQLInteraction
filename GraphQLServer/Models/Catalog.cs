using HotChocolate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Models
{
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

    }
}
