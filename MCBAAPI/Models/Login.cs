using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class Login : IModel
{
    [Key]
    [Required]
    [RegularExpression(@"^[0-9]{8}$",
        ErrorMessage = "Must be 8 digits")]
    [Display(Name = "Login ID")]
    public string LoginID { get; set; }

    [Required]
    [ForeignKey("Customer")]
    public int CustomerID { get; set; }

    [Required]
    [StringLength(64, MinimumLength = 64)]
    [Display(Name = "Password")]
    public string PasswordHash { get; set; }

    public virtual Customer Customer { get; set; }
}
