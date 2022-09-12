using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class Transaction : IModel
{
    [Key]
    [Required]
    public int TransactionID { get; set; }

    [Required]
    [RegularExpression(@"^D|W|T|S|B$",
        ErrorMessage = "Must be one of 'D', 'W', 'T', 'S', or 'B'")]
    [Display(Name = "Transaction Type")]
    public char TransactionType { get; set; }

    [Required]
    [ForeignKey("Account")]
    [Display(Name = "Account No.")]
    public int AccountNumber { get; set; }

    [ForeignKey("DestinationAccount")]
    [Display(Name = "Destination Account No.")]
    public int? DestinationAccountNumber { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [RegularExpression(@"^(?=.*[1-9]+)([0-9]*(\.[0-9]{0,2})?)$",
        ErrorMessage = "Must be positive with a maximum of 2 decimal places")]
    [Range(0,1000000,ErrorMessage = "MCBA users don't have that much money")]
    [DataType(DataType.Currency)]
    public decimal Amount { get; set; }

    [StringLength(30)]
    public string Comment { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
    [Display(Name = "Transaction Time")]
    public DateTime TransactionTimeUtc { get; set; }

    public virtual Account Account { get; set; }
    public virtual Account DestinationAccount { get; set; }
}