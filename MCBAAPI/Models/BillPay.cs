using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class BillPay : IModel
{
    [Key]
    [Required]
    public int BillPayID { get; set; }

    [Required]
    [ForeignKey("Account")]
    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Required]
    [ForeignKey("Payee")]
    [Display(Name = "Payee ID")]
    public int PayeeID { get; set; }

    [Required]
    [Column(TypeName = "money")]
    [RegularExpression(@"^(?=.*[1-9]+)([0-9]*(\.[0-9]{0,2})?)$",
        ErrorMessage = "Must be positive with a maximum of 2 decimal places")]
    [DataType(DataType.Currency)]
    public decimal Amount { set; get; }

    [Required]
    [Display(Name = "Scheduled Time")]
    public DateTime ScheduleTimeUtc { set; get; }

    [Required]
    [Column(TypeName = "char")]
    [RegularExpression(@"^O|M$",
        ErrorMessage = "Must be either 'O' or 'M'")]
    public char Period { set; get; }

    [Required]
    public int PaymentsDue { get; set; }

    [Required]
    public bool Frozen { get; set; }

    [Required]
    public bool Cancelled { get; set; }

    public virtual Payee Payee { get; set; }
}
