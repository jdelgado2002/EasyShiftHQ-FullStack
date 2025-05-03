using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Volo.Abp;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Identity;
using Volo.Abp.Emailing;
using Volo.Abp.Modularity;

namespace easyshifthq.Invitations;

public abstract class InvitationAppServiceTests<TStartupModule> : easyshifthqApplicationTestBase<TStartupModule> 
    where TStartupModule : IAbpModule
{

    private readonly IInvitationAppService _invitationAppService;

    protected InvitationAppServiceTests()
    {
        _invitationAppService = GetRequiredService<IInvitationAppService>();
    }

    [Fact]
    public async Task Should_Create_Valid_Invitation()
    {
        // Arrange
        var input = new CreateInvitationDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "employee"
        };

        // Act
        var result = await _invitationAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(input.Email);
        result.FirstName.ShouldBe(input.FirstName);
        result.LastName.ShouldBe(input.LastName);
        result.Role.ShouldBe(input.Role);
        result.Status.ShouldBe(InvitationStatus.Pending);
    }

    [Fact]
    public async Task Should_Not_Create_Invitation_For_Existing_User()
    {
        // Arrange
        var user = new IdentityUser(Guid.NewGuid(), "existing@example.com", "existing@example.com");
        await GetRequiredService<IdentityUserManager>().CreateAsync(user);

        var input = new CreateInvitationDto
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "employee"
        };

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.CreateAsync(input);
        });
    }

    [Fact]
    public async Task Should_Not_Create_Duplicate_Pending_Invitation()
    {
        // Arrange
        var input = new CreateInvitationDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Role = "employee"
        };

        await _invitationAppService.CreateAsync(input);

        // Act & Assert
        await Should.ThrowAsync<UserFriendlyException>(async () =>
        {
            await _invitationAppService.CreateAsync(input);
        });
    }
}