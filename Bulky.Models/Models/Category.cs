﻿using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }

        public int DisplayOrder { get; set; }
    }
}
