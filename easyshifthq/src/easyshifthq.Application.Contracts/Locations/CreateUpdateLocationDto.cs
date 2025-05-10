using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Locations;

public class CreateUpdateLocationDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
    
    [Required]
    public string Address { get; set; }
    
    [Required]
    public string TimeZone { get; set; }
    
    [StringLength(50)]
    public string? JurisdictionCode { get; set; }
    
    [StringLength(500)]
    public string? Notes { get; set; }
}