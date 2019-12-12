using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class CreateStorageView
    {
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Street")]
        public string Street { get; set; }
    }
}
