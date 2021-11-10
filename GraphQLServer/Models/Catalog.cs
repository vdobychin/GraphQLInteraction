﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQLServer.Models
{
    public class Catalog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Catalog()
        {

        }
        public Catalog(string name)
        {
            Name = name;
        }

    }
}