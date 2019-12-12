using System.ComponentModel.DataAnnotations;

namespace Store.ViewModels
{
    public class ChangePasswordView
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password", ErrorMessage = "Passwords don't match!!!")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
