﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace littlebreadloaf.Data
{
    public class Product
    {
        [Key]
        public Guid? ProductID { get; set; }
        
        [Display(Name = "Name", Prompt = "Enter name", Description = "The name of the product")]
        [StringLength(45, MinimumLength = 2)]
        [Required(ErrorMessage = "A product name is required.")]
        public string Name { get; set; }

        [Display(Name = "Description", Prompt = "Enter description", Description = "The description of the product")]
        [StringLength(255, MinimumLength = 2)]
        [Required(ErrorMessage = "A product description is required.")]
        public string Description { get; set; }

        [Display(Name = "Price", Prompt = "Enter price", Description = "The price of the product")]
        [Range(typeof(decimal), "0.01", "1000")]
        [Required(ErrorMessage = "A product price is required.")]
        public decimal? Price { get; set; }

        [Display(Name = "Unit", Prompt = "Enter Unit (e.g. loaf, roll, bagel)", Description = "The Unit of the product")]
        [StringLength(45, MinimumLength = 2)]
        [Required(ErrorMessage = "A product Unit is required.")]
        public string Unit { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? Created { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dddd, dd MMMM yyyy HH:mm:ss}")]
        public DateTime? LastUpdated { get; set; }

        [Display(Name = "Sort Order", Prompt = "Enter sort order", Description = "The order in which the product will be sorted in the catalogue")]
        public int SortOrder { get; set; }

        [Display(Name = "Show on front page", Prompt = "Show on page?", Description = "Determines whether the product should be shown on the main page")]
        public bool ShowOnFrontPage { get; set; }

        [Display(Name = "Pre Order", Prompt = "Can be pre-ordered?", Description = "Determines whether the product can be pre-ordered")]
        public bool PreOrder { get; set; }
    }
}
