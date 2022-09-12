using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class Account : IModel
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [RegularExpression(@"^[0-9]{4}",
        ErrorMessage = "Must be 4 digits")]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Required]
    [Column(TypeName = "char")]
    [RegularExpression(@"^C|S$",
        ErrorMessage = "Must be either 'S' or 'C'")]
    [Display(Name = "Account Type")]
    public char AccountType { get; set; }

    [Required]
    [ForeignKey("Customer")]
    public int CustomerID { get; set; }

    [InverseProperty("Account")]
    public virtual List<Transaction> Transactions { get; set; }

    public virtual Customer Customer { get; set; }
}
