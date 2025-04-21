using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SendGrid;
using SendGrid.Helpers.Mail;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;

namespace easyshifthq.Invitations;

public class InvitationAppService : ApplicationService, IInvitationAppService
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPasswordHasher<Invitation> _passwordHasher;
    private readonly IdentityUserManager _userManager;
    private readonly IConfiguration _configuration;

    public InvitationAppService(
        IInvitationRepository invitationRepository,
        IPasswordHasher<Invitation> passwordHasher,
        IdentityUserManager userManager,
        IConfiguration configuration)
    {
        _invitationRepository = invitationRepository;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<InvitationDto> CreateAsync(CreateInvitationDto input)
    {
        // Check if email already exists
        if (await _invitationRepository.EmailExistsAsync(input.Email))
        {
            throw new UserFriendlyException("An invitation for this email already exists.");
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
            catch (Exception)
            {
                // Log error but continue with other invitations
                continue;
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
        
        // Generate new token
        var token = Guid.NewGuid().ToString("N");
        var tokenHash = _passwordHasher.HashPassword(invitation, token);

        // Update invitation with new token
        invitation.SetTokenHash(tokenHash);
        await _invitationRepository.UpdateAsync(invitation);

        await SendInvitationEmailAsync(invitation, token);
    }

    public async Task<InvitationDto> VerifyTokenAsync(string token)
    {
        var invitations = await _invitationRepository.GetListAsync();
        var invitation = invitations.FirstOrDefault(x => 
            _passwordHasher.VerifyHashedPassword(null, x.TokenHash, token) == PasswordVerificationResult.Success);

        if (invitation == null)
        {
            throw new UserFriendlyException("Invalid or expired invitation token.");
        }

        if (invitation.IsExpired())
        {
            throw new UserFriendlyException("This invitation has expired.");
        }

        if (invitation.Status != InvitationStatus.Pending)
        {
            throw new UserFriendlyException("This invitation is no longer valid.");
        }

        return ObjectMapper.Map<Invitation, InvitationDto>(invitation);
    }

    private async Task SendInvitationEmailAsync(Invitation invitation, string token)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var msg = new SendGridMessage()
        {
            From = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]),
            Subject = "You've been invited to join EasyShiftHQ",
            PlainTextContent = $"You've been invited to join {CurrentTenant.Name} on EasyShiftHQ. Click the link below to set up your account: {_configuration["App:SelfUrl"]}/accept-invitation?token={token}"
        };
        
        msg.AddTo(new EmailAddress(invitation.Email));

        await client.SendEmailAsync(msg);
    }
}