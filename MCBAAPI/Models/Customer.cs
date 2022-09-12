using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAAPI.Models;

public class Customer : IModel
{
    [Key]
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [RegularExpression(@"^[0-9]{4}$",
        ErrorMessage = "Must be 4 digits")]
    public int CustomerID { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [RegularExpression(@"^[0-9]{3}\s[0-9]{3}\s[0-9]{3}$",
        ErrorMessage = "TFN must be numbers in the format 'XXX XXX XXX' including spaces")]
    [Display(Name = "Tax File Number")]
    public string TFN { get; set; }

    [StringLength(50)]
    public string Address { get; set; }

    [StringLength(40)]
    public string City { get; set; }

    [RegularExpression(@"^ACT|NSW|NT|QLD|SA|TAS|VIC|WA$",
        ErrorMessage = "Must be one of 'ACT', 'NSW', 'NT', 'QLD', 'SA', 'TAS', 'VIC', or 'WA'")]
    public string State { get; set; }

    [DataType(DataType.PostalCode,
        ErrorMessage = "Must be 4 digits")]
    [RegularExpression((@"^\d{4}$"))]
    [Display(Name = "Post Code")]
    public string PostCode { get; set; }

    [RegularExpression(@"^04[0-9]{2}\s[0-9]{3}\s[0-9]{3}$",
        ErrorMessage = "Mobile number must be of the format '04XX XXX XXX' including spaces")]
    public string Mobile { get; set; }

    [Required]
    public bool Frozen { get; set; }
    
    public List<Account> Accounts { get; set; }
}
