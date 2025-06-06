using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Locations;

public class CreateUpdateLocationDto
{
    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; }

    [Required]
    [StringLength(50)]
    public string TimeZone { get; set; }
    
    [MaxLength(LocationConsts.MaxJurisdictionCodeLength)]
    public string? JurisdictionCode { get; set; }
    
    [MaxLength(LocationConsts.MaxNotesLength)]
    public string? Notes { get; set; }
}