using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class Payee : IModel
{
    [Key]
    [Required]
    public int PayeeID { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Payee Name")]
    public string Name { get; set; }

    [Required]
    [StringLength(50)]
    public string Address { get; set; }

    [Required]
    [StringLength(40)]
    public string City { get; set; }

    [Required]
    [RegularExpression(@"^ACT|NSW|NT|QLD|SA|TAS|VIC|WA$",
        ErrorMessage = "Must be one of 'ACT', 'NSW', 'NT', 'QLD', 'SA', 'TAS', 'VIC', or 'WA'")]
    public string State { get; set; }

    [Required]
    [DataType(DataType.PostalCode,
        ErrorMessage = "Must be 4 digits")]
    [Display(Name = "Post Code")]
    public string PostCode { get; set; }

    [Required]
    [RegularExpression(@"^\(0[0-9]\)\s[0-9]{4}\s[0-9]{4}$",
        ErrorMessage = "Must follow format '(0X) XXXX XXXX' including spaces and parenthesis")]
    public string Phone { get; set; }

    public virtual List<BillPay> BillPays{ get; set; }
}
