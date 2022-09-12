using System.ComponentModel.DataAnnotations;

namespace SupportLibrary.ViewModels;

public class LoginViewModel
{
    [Required]
    [RegularExpression(@"^[0-9]{8}$",
        ErrorMessage = "Must be 8 digits")]
    [Display(Name = "Login ID")]
    public string LoginID { get; set; }

    [RegularExpression(@"^[0-9]{4}$",
        ErrorMessage = "Must be 4 digits")]
    public int CustomerID { get; set; }

    [Required]
    [Display(Name = "Password")]
    public string Password { get; set; }

    public CustomerViewModel Customer { get; set; }
}
