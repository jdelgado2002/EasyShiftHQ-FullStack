using System;
using System.Threading.Tasks;
using easyshifthq.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.TextTemplating;

namespace easyshifthq.Availabilities;

/// <summary>
/// Handles time off request notifications and sends them to managers.
/// </summary>
public class TimeOffRequestNotificationHandler : 
    IDistributedEventHandler<TimeOffRequestedEto>,
    IDistributedEventHandler<TimeOffApprovedEto>,
    IDistributedEventHandler<TimeOffDeniedEto>,
    ITransientDependency
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<TimeOffRequestNotificationHandler> _logger;
    private readonly ISettingProvider _settingProvider;

    public TimeOffRequestNotificationHandler(
        IIdentityUserRepository userRepository,
        IEmailSender emailSender,
        ILogger<TimeOffRequestNotificationHandler> logger,
        ISettingProvider settingProvider)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _logger = logger;
        _settingProvider = settingProvider;
    }

    public async Task HandleEventAsync(TimeOffRequestedEto eventData)
    {
        try
        {
            // Log the time off request
            _logger.LogInformation(
                "Time off request submitted - Employee: {EmployeeName} ({EmployeeId}), " +
                "Period: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}, Reason: {Reason}",
                eventData.EmployeeName,
                eventData.EmployeeId,
                eventData.StartDate,
                eventData.EndDate,
                eventData.Reason ?? "Not provided");

            // In a real implementation, we would query actual managers
            // For now, we'll send to a manager email from settings
            var managerEmail = await _settingProvider.GetOrNullAsync("App.ManagerEmail") ?? "manager@easyshifthq.com";
            
            // Send email to manager about new time off request
            await _emailSender.SendAsync(
                managerEmail,
                "Time Off Request Needs Approval",
                $"<h2>New Time Off Request</h2>" +
                $"<p>Employee: <strong>{eventData.EmployeeName}</strong></p>" +
                $"<p>Period: <strong>{eventData.StartDate:d} to {eventData.EndDate:d}</strong></p>" +
                $"<p>Total Days: <strong>{(eventData.EndDate.Date - eventData.StartDate.Date).Days + 1}</strong></p>" +
                $"<p>Reason: <strong>{(eventData.Reason ?? "Not provided")}</strong></p>" +
                $"<p>Please review this request in the <a href='https://app.easyshifthq.com/availabilities/manager-view/{eventData.EmployeeId}'>EasyShiftHQ Manager Portal</a>.</p>");
            
            _logger.LogInformation(
                "Time off request notification sent to manager: {ManagerEmail}",
                managerEmail);
        }
        catch (Exception ex)
        {
            // Log any errors but don't rethrow - we don't want to fail the entire operation
            // if notifications aren't sent
            _logger.LogError(ex, "Error processing time off request notification");
        }
    }
    
    public async Task HandleEventAsync(TimeOffApprovedEto eventData)
    {
        try
        {
            // Get employee's email
            var employee = await _userRepository.GetAsync(eventData.EmployeeId);
            
            // Get approver's name
            var approver = await _userRepository.GetAsync(eventData.ApproverId);
            string approverName = approver.Name ?? approver.UserName ?? "Manager";

            // Send approval notification to employee
            await _emailSender.SendAsync(
                employee.Email,
                "Your Time Off Request Has Been Approved",
                $"<h2>Time Off Request Approved</h2>" +
                $"<p>Dear {employee.Name},</p>" +
                $"<p>Your request for time off from <strong>{eventData.StartDate:d}</strong> to <strong>{eventData.EndDate:d}</strong> has been <strong>approved</strong>.</p>" +
                $"<p>Your request was approved by {approverName} on {eventData.ApprovalDate:g}.</p>" +
                $"<p>You can view your approved time off in the <a href='https://app.easyshifthq.com/availabilities/employee'>EasyShiftHQ Portal</a>.</p>" +
                $"<p>Thank you for using EasyShiftHQ!</p>");
            
            _logger.LogInformation(
                "Time off approval notification sent to employee: {EmployeeEmail}",
                employee.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing time off approval notification");
        }
    }
    
    public async Task HandleEventAsync(TimeOffDeniedEto eventData)
    {
        try
        {
            // Get employee's email
            var employee = await _userRepository.GetAsync(eventData.EmployeeId);
            
            // Get approver's name
            var approver = await _userRepository.GetAsync(eventData.ApproverId);
            string approverName = approver.Name ?? approver.UserName ?? "Manager";

            // Send denial notification to employee
            await _emailSender.SendAsync(
                employee.Email,
                "Your Time Off Request Has Been Declined",
                $"<h2>Time Off Request Declined</h2>" +
                $"<p>Dear {employee.Name},</p>" +
                $"<p>Your request for time off from <strong>{eventData.StartDate:d}</strong> to <strong>{eventData.EndDate:d}</strong> has been <strong>declined</strong>.</p>" +
                $"<p>Reason for denial: {eventData.DenialReason}</p>" +
                $"<p>Your request was reviewed by {approverName} on {eventData.DenialDate:g}.</p>" +
                $"<p>If you have any questions, please contact your manager directly.</p>" +
                $"<p>You can view your time off requests in the <a href='https://app.easyshifthq.com/availabilities/employee'>EasyShiftHQ Portal</a>.</p>");
            
            _logger.LogInformation(
                "Time off denial notification sent to employee: {EmployeeEmail}",
                employee.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing time off denial notification");
        }
    }
}
