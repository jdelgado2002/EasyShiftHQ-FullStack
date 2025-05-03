using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using System.Net;
using Microsoft.Extensions.Logging;
using Volo.Abp.Emailing;

namespace easyshifthq.Invitations;

public class InvitationAppService : ApplicationService, IInvitationAppService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPasswordHasher<Invitation> _passwordHasher;
    private readonly IdentityUserManager _userManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailSender _emailSender;

    public InvitationAppService(
        IInvitationRepository invitationRepository,
        IPasswordHasher<Invitation> passwordHasher,
        IdentityUserManager userManager,
        IConfiguration configuration,
        IEmailSender emailSender)
    {
        _invitationRepository = invitationRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _configuration = configuration;
        _emailSender = emailSender;
    }

    public async Task<InvitationDto> CreateAsync(CreateInvitationDto input)
    {
        // Check if email already exists as a user
        var existingUser = await _userManager.FindByEmailAsync(input.Email);
        if (existingUser != null)
        {
            throw new UserFriendlyException(L["UserAlreadyExists"]);
        }

        // Check if a pending invitation exists
        if (await _invitationRepository.GetListAsync(x => x.Email == input.Email && x.Status == InvitationStatus.Pending) is { } pendingInvites && pendingInvites.Any())
        {
            throw new UserFriendlyException(L["PendingInvitationExists"]);
        }

        // Generate secure token
        var token = Guid.NewGuid().ToString("N");
        var invitation = new Invitation(
            GuidGenerator.Create(),
            input.Email,
            input.FirstName,
            input.LastName,
            input.Role,
            input.LocationId,
            string.Empty,
            CurrentTenant.Id
        );
        var tokenHash = _passwordHasher.HashPassword(invitation, token);
        invitation.SetTokenHash(tokenHash);

        await _invitationRepository.InsertAsync(invitation);

        await SendInvitationEmailAsync(invitation, token);

        return ObjectMapper.Map<Invitation, InvitationDto>(invitation);
    }

    public async Task<List<InvitationDto>> CreateBulkAsync(BulkInvitationDto input)
    {
        var results = new List<InvitationDto>();

        foreach (var inviteDto in input.Invitations)
        {
            try
            {
                var invitation = await CreateAsync(inviteDto);
                results.Add(invitation);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to create invitation for {Email}", inviteDto.Email);
            }
        }

        return results;
    }

    public async Task<List<InvitationDto>> GetPendingAsync()
    {
        var invitations = await _invitationRepository.GetPendingInvitationsAsync();
        return ObjectMapper.Map<List<Invitation>, List<InvitationDto>>(invitations);
    }

    public async Task<InvitationDto> AcceptAsync(Guid id)
    {
        var invitation = await _invitationRepository.GetAsync(id);
        
        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new UserFriendlyException(L["InvitationNotPending"]);
        }

        if (invitation.IsExpired())
        {
            throw new UserFriendlyException(L["InvitationExpired"]);
        }

        invitation.Accept();
        await _invitationRepository.UpdateAsync(invitation);

        return ObjectMapper.Map<Invitation, InvitationDto>(invitation);
    }

    public async Task RevokeAsync(Guid id)
    {
        var invitation = await _invitationRepository.GetAsync(id);
        invitation.Revoke();
        await _invitationRepository.UpdateAsync(invitation);
    }

    public async Task ResendAsync(Guid id)
    {
        var invitation = await _invitationRepository.GetAsync(id);
        
        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new UserFriendlyException(L["CannotResendNonPendingInvitation"]);
        }

        // Generate new token
        var token = Guid.NewGuid().ToString("N");
        var tokenHash = _passwordHasher.HashPassword(invitation, token);

        // Update invitation with new token and reset expiration
        invitation.SetTokenHash(tokenHash);
        invitation.SetExpiresAt(DateTime.UtcNow.AddDays(7));
        await _invitationRepository.UpdateAsync(invitation);

        await SendInvitationEmailAsync(invitation, token);
    }

    public async Task<InvitationDto> VerifyTokenAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new UserFriendlyException(L["InvalidToken"]);
        }

        var invitations = await _invitationRepository.GetListAsync();
        var invitation = invitations.FirstOrDefault(x => 
            _passwordHasher.VerifyHashedPassword(x, x.TokenHash, token) == PasswordVerificationResult.Success);

        if (invitation == null)
        {
            throw new UserFriendlyException(L["InvalidToken"]);
        }

        if (invitation.IsExpired())
        {
            throw new UserFriendlyException(L["InvitationExpired"]);
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new UserFriendlyException(L["InvitationNotPending"]);
        }

        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(invitation.Email);
        if (existingUser != null)
        {
            throw new UserFriendlyException(L["UserAlreadyExists"]);
        }

        return ObjectMapper.Map<Invitation, InvitationDto>(invitation);
    }

    private async Task SendInvitationEmailAsync(Invitation invitation, string token)
    {
        var acceptUrl = $"{_configuration["App:SelfUrl"]}/acceptinvitation?token={WebUtility.UrlEncode(token)}";
        var isSSOEnabled = _configuration.GetValue<bool>("Authentication:SSO:Enabled");

        var emailTemplate = isSSOEnabled ? 
            $"You've been invited to join {CurrentTenant.Name} on EasyShiftHQ. Click the link below to set up your account using your organization credentials:\n\n{acceptUrl}" :
            $"You've been invited to join {CurrentTenant.Name} on EasyShiftHQ. Click the link below to set up your account and create your password:\n\n{acceptUrl}";

        try
        {
            await _emailSender.SendAsync(
                invitation.Email,
                "You've been invited to join EasyShiftHQ",
                emailTemplate
            );
        }
        catch (Exception ex)
        {
            Logger.LogWarning(ex, "Failed to send invitation email to {Email}", invitation.Email);
            // Don't throw - we still want to create the invitation even if email fails
        }
    }
}