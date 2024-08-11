using System.ComponentModel.DataAnnotations;

namespace Contracts;

public class LeagueModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name length can't be more than 50.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; }

    [Range(1900, 2100, ErrorMessage = "Season must be between 1900 and 2100.")]
    public int Season { get; set; }
}