using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SupportLibrary.ViewModels;

public class BillPayViewModel
{
    public int BillPayID { get; set; }

    [Required]
    public int AccountNumber { get; set; }

    // using a string allows input of payee name in SelectList
    [Required]
    [Display(Name = "Payee")]
    public string PayeeName { get; set; }

    public int PayeeID { get; set; }

    // making this a string prevents overflow for inputs above decimal maximum
    [Required(ErrorMessage = "Required field")]
    [RegularExpression(@"^(?=.*[1-9]+)([0-9]*(\.[0-9]{0,2})?)$",
        ErrorMessage = "Must be positive number with a maximum of 2 decimal places")]
    [Display(Name = "Amount")]
    public string Amount { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Scheduled Time")]
    public DateTime ScheduleTimeUtc { set; get; }

    [Required]
    public char Period { set; get; }

    [Required]
    [Display(Name = "Payments Due")]
    public int PaymentsDue { get; set; }

    [Required]
    public bool Frozen { get; set; }

    [Required]
    public bool Cancelled { get; set; }
    
    public SelectList Payees { get; set; }
}
