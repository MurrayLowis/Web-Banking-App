using System.ComponentModel.DataAnnotations;

namespace SupportLibrary.ViewModels;

public class CustomerViewModel
{
    [Required]
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

    [RegularExpression(@"^ACT|NSW|NT|QLD|SA|TAS|VIC|WA|act|nsw|nt|qld|sa|tas|vic|wa$",
        ErrorMessage = "Must be one of 'ACT', 'NSW', 'NT', 'QLD', 'SA', 'TAS', 'VIC', or 'WA'")]
    public string State { get; set; }
    
    [RegularExpression(@"^[0-9]{4}$",
        ErrorMessage = "Must be 4 digits")]
    [Display(Name = "Post Code")]
    public string PostCode { get; set; }

    [RegularExpression(@"^04[0-9]{2}\s[0-9]{3}\s[0-9]{3}$",
        ErrorMessage = "Mobile number must be of the format '04XX XXX XXX' including spaces")]
    public string Mobile { get; set; }

    [Required]
    public bool Frozen { get; set; }
        
    public List<AccountViewModel> Accounts { get; set; }
    
}
