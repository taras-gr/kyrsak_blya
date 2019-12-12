using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace Store.ViewModels
{
    public class CreateGoodView
    {
        public CreateGoodView()
        {
            Storages = new List<Storage>();
            Producers = new List<Producer>();
        }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Specification")]
        [DataType(DataType.MultilineText)]
        public string Specification { get; set; }

        [Required]
        [Display(Name = "Photo Url")]
        public string PhotoUrl { get; set; }

        [Required]
        [Display(Name = "Year of goods production")]
        public int YearOfManufacture { get; set; }

        [Required]
        [Display(Name = "The warranty period")]
        public int WarrantyTerm { get; set; }

        [Required]
        [Display(Name = "Producer")]
        public List<Producer> Producers { get; set; }

        [Required]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Product type")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Amount")]
        public int Count { get; set; }

        public List<Storage> Storages { get; set; }
    }
}
