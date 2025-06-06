using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace easyshifthq.Locations;
using System.ComponentModel.DataAnnotations;
using easyshifthq.Locations;

 public class LocationDto : EntityDto<Guid>, IMultiTenant
 {
     public Guid? TenantId { get; set; }
     public string Name { get; set; }
     public string Address { get; set; }
     public bool IsActive { get; set; }
     public string TimeZone { get; set; }
    [MaxLength(LocationConsts.MaxJurisdictionCodeLength)]
     public string JurisdictionCode { get; set; }
    [MaxLength(LocationConsts.MaxNotesLength)]
     public string Notes { get; set; }
}