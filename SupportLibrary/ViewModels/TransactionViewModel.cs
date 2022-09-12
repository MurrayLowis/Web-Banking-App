using System.ComponentModel.DataAnnotations;
using SupportLibrary.ViewModels;

namespace SupportLibrary.ViewModels;

public class TransactionViewModel
{
    [Required]
    public int TransactionID { get; set; }

    [Required]
    [RegularExpression(@"^D|W|T|S|B$",
        ErrorMessage = "Must be one of 'D', 'W', 'T', 'S', or 'B'")]
    [Display(Name = "Transaction Type")]
    public char TransactionType { get; set; }

    [Required]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [RegularExpression(@"^[0-9]{4}",
        ErrorMessage = "Must be 4 digits")]
    [Display(Name = "Destination Account Number")]
    public int? DestinationAccountNumber { get; set; }
    
    // making this a string prevents overflow for inputs above decimal maximum
    [Required(ErrorMessage = "Required field")]
    [RegularExpression(@"^(?=.*[1-9]+)([0-9]*(\.[0-9]{0,2})?)$",
        ErrorMessage = "Must be positive number with a maximum of 2 decimal places")]
    [Display(Name = "Amount")]
    public string Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Transaction Time")]
    public DateTime TransactionTimeUtc { get; set; }
    public virtual AccountViewModel Account { get; set; }
}
