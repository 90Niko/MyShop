﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyShop.Data.Models
{
    [Comment("Product")]
    public class Product
    {
        [Key]
        [Comment("PK of Product")]
        public int Id { get; set; }

        [Required]
        [MaxLength(Constants.Constants.ProductNameMax)]
        [Comment("Name of Product")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Comment("Price of Product")]
        public decimal Price { get; set; }

        [Required]
        [Comment("This is the date the product was created")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Comment("Product Category identifier")]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("CategoryId")]
        public Category Category { get; set; } = null!;

        [Required]
        [MaxLength(Constants.Constants.ProductDescriptionMax)]
        [Comment("Description of Product")]
        public string Description { get; set; } = string.Empty;

        [Comment("Image path or URL of the product")]
        public string? ImagePath { get; set; }
    }
}
