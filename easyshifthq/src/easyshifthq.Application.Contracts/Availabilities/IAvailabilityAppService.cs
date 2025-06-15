using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace easyshifthq.Availabilities;

public interface IAvailabilityAppService : IApplicationService
{
    [HttpGet]
    Task<AvailabilityDto> GetAsync(Guid id);
    
    [HttpGet]
    Task<PagedResultDto<AvailabilityDto>> GetListAsync(GetAvailabilityListDto input);
    
    [HttpGet]
    Task<List<AvailabilityDto>> GetEmployeeWeeklyAvailabilityAsync(Guid employeeId);
    
    [HttpGet]
    Task<List<AvailabilityDto>> GetEmployeeTimeOffRequestsAsync(Guid employeeId);
    
    [HttpPost]
    Task<AvailabilityDto> SubmitWeeklyAvailabilityAsync(SubmitWeeklyAvailabilityDto input);
    
    [HttpPost]
    Task<AvailabilityDto> SubmitTimeOffRequestAsync(SubmitTimeOffRequestDto input);
    
    [HttpPut]
    Task<AvailabilityDto> UpdateWeeklyAvailabilityAsync(Guid id, SubmitWeeklyAvailabilityDto input);
    
    [HttpPut]
    Task<AvailabilityDto> UpdateTimeOffRequestAsync(Guid id, SubmitTimeOffRequestDto input);
    
    [HttpPut]
    [Route("{id}/approve")]
    Task<AvailabilityDto> ApproveTimeOffRequestAsync(Guid id);
    
    [HttpPut]
    [Route("{id}/deny")]
    Task<AvailabilityDto> DenyTimeOffRequestAsync(Guid id, string reason);
    
    [HttpDelete]
    Task DeleteAsync(Guid id);

    [HttpGet]
    Task<List<AvailabilityDto>> GetCurrentUserWeeklyAvailabilityAsync();
}
