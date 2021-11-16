using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Models
{
    /// <summary>
    /// Подкаталог
    /// </summary>
    public class SubCatalog
    {
        /// <summary>
        /// Уникальный идентификатор
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Имя подкаталога
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
