using System;
using System.Threading.Tasks;
using easyshifthq.Invitations;
using Microsoft.AspNetCore.Identity;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Xunit;

namespace easyshifthq.Invitations;

public abstract class InvitationSecurityTests<TStartupModule> : easyshifthqApplicationTestBase<TStartupModule> 
    where TStartupModule : IAbpModule

{
    private readonly IInvitationAppService _invitationAppService;
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPasswordHasher<Invitation> _passwordHasher;
    private readonly IdentityUserManager _userManager;

    protected InvitationSecurityTests()
    {
        _invitationAppService = GetRequiredService<IInvitationAppService>();
        _invitationRepository = GetRequiredService<IInvitationRepository>();
        _passwordHasher = GetRequiredService<IPasswordHasher<Invitation>>();
        _userManager = GetRequiredService<IdentityUserManager>();
    }

    [Fact]
    public async Task Should_Verify_Valid_Token()
    {
        // Arrange
        var invitation = await CreateTestInvitation();
        var token = GenerateTokenForInvitation(invitation);

        // Act
        var result = await _invitationAppService.VerifyTokenAsync(token);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(invitation.Id);
        result.Status.ShouldBe(InvitationStatus.Pending);
    }

    [Fact]
    public async Task Should_Not_Verify_Invalid_Token()
    {
        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.VerifyTokenAsync("invalid-token");
        });
    }

    [Fact]
    public async Task Should_Not_Verify_Expired_Token()
    {
        // Arrange
        var invitation = await CreateTestInvitation();
        await WithUnitOfWorkAsync(async () =>
        {
            // Set expiration to 1 second in the future
            invitation.SetExpiresAt(DateTime.UtcNow.AddSeconds(1));
            await _invitationRepository.UpdateAsync(invitation);
        });
        var token = GenerateTokenForInvitation(invitation);

        // Wait for the invitation to expire
        await Task.Delay(1500); // Wait 1.5 seconds to ensure expiration

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.VerifyTokenAsync(token);
        });
    }

    [Fact]
    public async Task Should_Not_Verify_Revoked_Token()
    {
        // Arrange
        var invitation = await CreateTestInvitation();
        var token = GenerateTokenForInvitation(invitation);
        await _invitationAppService.RevokeAsync(invitation.Id);

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.VerifyTokenAsync(token);
        });
    }

    [Fact]
    public async Task Should_Not_Verify_Token_For_Existing_User()
    {
        // Arrange
        var invitation = await CreateTestInvitation();
        var token = GenerateTokenForInvitation(invitation);
        
        // Create user with same email
        var user = new IdentityUser(Guid.NewGuid(), invitation.Email, invitation.Email);
        await _userManager.CreateAsync(user);

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.VerifyTokenAsync(token);
        });
    }

    [Fact]
    public async Task Should_Generate_New_Token_On_Resend()
    {
        // Arrange
        var invitation = await CreateTestInvitation();
        var originalToken = GenerateTokenForInvitation(invitation);

        // Act
        await _invitationAppService.ResendAsync(invitation.Id);

        // Assert
        var updatedInvitation = await _invitationRepository.GetAsync(invitation.Id);
        _passwordHasher.VerifyHashedPassword(
            updatedInvitation, 
            updatedInvitation.TokenHash, 
            originalToken
        ).ShouldBe(PasswordVerificationResult.Failed);
    }

    private async Task<Invitation> CreateTestInvitation()
    {
        var input = new CreateInvitationDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = "employee",
            LocationId = Guid.NewGuid()
        };

        var result = await _invitationAppService.CreateAsync(input);
        return await _invitationRepository.GetAsync(result.Id);
    }

    private string GenerateTokenForInvitation(Invitation invitation)
    {
        var token = Guid.NewGuid().ToString("N");
        var hash = _passwordHasher.HashPassword(invitation, token);
        invitation.SetTokenHash(hash);
        
        WithUnitOfWorkAsync(async () =>
        {
            await _invitationRepository.UpdateAsync(invitation);
        }).GetAwaiter().GetResult();

        return token;
    }
}