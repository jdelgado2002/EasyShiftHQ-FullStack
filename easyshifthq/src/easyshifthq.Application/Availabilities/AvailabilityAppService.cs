using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using easyshifthq.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Timing;
using Volo.Abp.Users;

namespace easyshifthq.Availabilities;

[Authorize]
public class AvailabilityAppService : ApplicationService, IAvailabilityAppService
{
    private readonly IRepository<Availability, Guid> _availabilityRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDistributedEventBus _eventBus;
    private readonly IClock _clock;

    public AvailabilityAppService(
        IRepository<Availability, Guid> availabilityRepository,
        ICurrentUser currentUser,
        IDistributedEventBus eventBus,
        IClock clock)
    {
        _availabilityRepository = availabilityRepository;
        _currentUser = currentUser;
        _eventBus = eventBus;
        _clock = clock;
    }

    [Authorize(AvailabilityPermissions.Availabilities.Default)]
    public async Task<AvailabilityDto> GetAsync(Guid id)
    {
        var availability = await _availabilityRepository.GetAsync(id);
        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize(AvailabilityPermissions.Availabilities.Default)]
    public async Task<PagedResultDto<AvailabilityDto>> GetListAsync(GetAvailabilityListDto input)
    {
        var query = (await _availabilityRepository.GetQueryableAsync())
            .WhereIf(input.EmployeeId.HasValue, a => a.EmployeeId == input.EmployeeId)
            .WhereIf(input.DayOfWeek.HasValue, a => a.DayOfWeek == input.DayOfWeek)
            .WhereIf(input.TimeOffStartDate.HasValue, a => a.TimeOffStartDate != null && a.TimeOffStartDate <= input.TimeOffStartDate)
            .WhereIf(input.TimeOffEndDate.HasValue, a => a.TimeOffEndDate != null && a.TimeOffEndDate >= input.TimeOffEndDate);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderBy(a => a.DayOfWeek)
            .ThenBy(a => a.StartTime)
            .ThenBy(a => a.TimeOffStartDate)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<AvailabilityDto>
        {
            TotalCount = totalCount,
            Items = ObjectMapper.Map<List<Availability>, List<AvailabilityDto>>(items)
        };
    }

    [Authorize]
    public async Task<List<AvailabilityDto>> GetEmployeeWeeklyAvailabilityAsync(Guid employeeId)
    {
        // Check if user is manager or employee themselves
        if (CurrentUser.Id != employeeId && !await AuthorizationService
                .IsGrantedAsync(AvailabilityPermissions.Availabilities.Default))
        {
            throw new UnauthorizedAccessException("You are not authorized to view this employee's availability.");
        }

        var query = (await _availabilityRepository.GetQueryableAsync())
            .Where(a => a.EmployeeId == employeeId)
            .Where(a => a.TimeOffStartDate == null && a.TimeOffEndDate == null)
            .OrderBy(a => a.DayOfWeek);

        var items = await AsyncExecuter.ToListAsync(query);
        return ObjectMapper.Map<List<Availability>, List<AvailabilityDto>>(items);
    }

    [Authorize]
    public async Task<List<AvailabilityDto>> GetEmployeeTimeOffRequestsAsync(Guid employeeId)
    {
        // Check if user is manager or employee themselves
        if (CurrentUser.Id != employeeId && !await AuthorizationService
                .IsGrantedAsync(AvailabilityPermissions.Availabilities.Default))
        {
            throw new UnauthorizedAccessException("You are not authorized to view this employee's time off requests.");
        }

        var query = (await _availabilityRepository.GetQueryableAsync())
            .Where(a => a.EmployeeId == employeeId)
            .Where(a => a.TimeOffStartDate != null && a.TimeOffEndDate != null)
            .OrderBy(a => a.TimeOffStartDate);

        var items = await AsyncExecuter.ToListAsync(query);
        return ObjectMapper.Map<List<Availability>, List<AvailabilityDto>>(items);
    }

    [Authorize]
    public async Task<AvailabilityDto> SubmitWeeklyAvailabilityAsync(SubmitWeeklyAvailabilityDto input)
    {
        // Use current user's ID as employee ID
        var employeeId = _currentUser.GetId();

        var availability = new Availability(
            GuidGenerator.Create(),
            employeeId,
            input.DayOfWeek,
            input.StartTime,
            input.EndTime,
            input.IsAvailable
        );

        await _availabilityRepository.InsertAsync(availability);

        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize]
    public async Task<AvailabilityDto> SubmitTimeOffRequestAsync(SubmitTimeOffRequestDto input)
    {
        // Use current user's ID as employee ID
        var employeeId = _currentUser.GetId();

        var availability = new Availability(
            GuidGenerator.Create(),
            employeeId,
            input.StartDate,
            input.EndDate,
            input.Reason
        );

        await _availabilityRepository.InsertAsync(availability);

        // After saving, publish event
        await _eventBus.PublishAsync(new TimeOffRequestedEto
        {
            AvailabilityId = availability.Id,
            EmployeeId = availability.EmployeeId,
            EmployeeName = _currentUser.Name ?? _currentUser.UserName ?? "Employee",
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Reason = input.Reason,
            CreationTime = _clock.Now
        });

        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize]
    public async Task<AvailabilityDto> UpdateWeeklyAvailabilityAsync(Guid id, SubmitWeeklyAvailabilityDto input)
    {
        var availability = await _availabilityRepository.GetAsync(id);

        // Check if user is the employee who created this or has edit permission
        if (availability.EmployeeId != _currentUser.GetId() && !await AuthorizationService
                .IsGrantedAsync(AvailabilityPermissions.Availabilities.Edit))
        {
            throw new UnauthorizedAccessException("You are not authorized to update this availability.");
        }

        availability.SetAvailability(
            input.DayOfWeek,
            input.StartTime,
            input.EndTime,
            input.IsAvailable
        );

        await _availabilityRepository.UpdateAsync(availability);

        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize]
    public async Task<AvailabilityDto> UpdateTimeOffRequestAsync(Guid id, SubmitTimeOffRequestDto input)
    {
        var availability = await _availabilityRepository.GetAsync(id);

        // Check if user is the employee who created this or has edit permission
        if (availability.EmployeeId != _currentUser.GetId() && !await AuthorizationService
                .IsGrantedAsync(AvailabilityPermissions.Availabilities.Edit))
        {
            throw new UnauthorizedAccessException("You are not authorized to update this time off request.");
        }

        availability.SetTimeOff(
            input.StartDate,
            input.EndDate,
            input.Reason
        );

        await _availabilityRepository.UpdateAsync(availability);

        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize(AvailabilityPermissions.Availabilities.Edit)]
    public async Task<AvailabilityDto> ApproveTimeOffRequestAsync(Guid id)
    {
        var availability = await _availabilityRepository.GetAsync(id);
        
        if (availability.TimeOffStartDate == null || availability.TimeOffEndDate == null)
        {
            throw new BusinessException("Availability:NotATimeOffRequest");
        }
        
        // Get current user ID for the approver
        var approverId = _currentUser.GetId();
        
        // Approve the time off request
        availability.ApproveTimeOff(approverId);
        
        await _availabilityRepository.UpdateAsync(availability);
        
        // After saving, publish event for notifications
        await _eventBus.PublishAsync(new TimeOffApprovedEto
        {
            AvailabilityId = availability.Id,
            EmployeeId = availability.EmployeeId,
            ApproverId = approverId,
            StartDate = availability.TimeOffStartDate.Value,
            EndDate = availability.TimeOffEndDate.Value,
            ApprovalDate = _clock.Now
        });
        
        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize(AvailabilityPermissions.Availabilities.Edit)]
    public async Task<AvailabilityDto> DenyTimeOffRequestAsync(Guid id, string reason)
    {
        var availability = await _availabilityRepository.GetAsync(id);
        
        if (availability.TimeOffStartDate == null || availability.TimeOffEndDate == null)
        {
            throw new BusinessException("Availability:NotATimeOffRequest");
        }
        
        // Get current user ID for the approver
        var approverId = _currentUser.GetId();
        
        // Deny the time off request with the reason
        availability.DenyTimeOff(approverId, reason);
        
        await _availabilityRepository.UpdateAsync(availability);
        
        // After saving, publish event for notifications
        await _eventBus.PublishAsync(new TimeOffDeniedEto
        {
            AvailabilityId = availability.Id,
            EmployeeId = availability.EmployeeId,
            ApproverId = approverId,
            StartDate = availability.TimeOffStartDate.Value,
            EndDate = availability.TimeOffEndDate.Value,
            DenialReason = reason,
            DenialDate = _clock.Now
        });
        
        return ObjectMapper.Map<Availability, AvailabilityDto>(availability);
    }

    [Authorize]
    public async Task DeleteAsync(Guid id)
    {
        var availability = await _availabilityRepository.GetAsync(id);

        // Check if user is the employee who created this or has delete permission
        if (availability.EmployeeId != _currentUser.GetId() && !await AuthorizationService
                .IsGrantedAsync(AvailabilityPermissions.Availabilities.Delete))
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this availability.");
        }

        await _availabilityRepository.DeleteAsync(id);
    }
}
