using System.ComponentModel.DataAnnotations;

namespace SupportLibrary.ViewModels;

public class AccountViewModel
{
    [RegularExpression(@"^[0-9]{4}",
        ErrorMessage = "Must be 4 digits")]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Required]
    [RegularExpression(@"^C|S$",
        ErrorMessage = "Must be either 'S' or 'C'")]
    [Display(Name = "Account Type")]
    public char AccountType { get; set; }

    [Required]
    public int CustomerID { get; set; }

    public List<TransactionViewModel> Transactions { get; set; }
}
