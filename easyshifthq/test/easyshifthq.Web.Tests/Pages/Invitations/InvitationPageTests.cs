using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using easyshifthq.Invitations;
using easyshifthq.Permissions;
using easyshifthq.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;
using Volo.Abp.Authorization.Permissions;

namespace easyshifthq.Web.Pages.Invitations;

public class InvitationPageTests : easyshifthqWebTestBase
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IPasswordHasher<Invitation> _passwordHasher;
    private readonly IInvitationAppService _invitationAppService;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly ICurrentPrincipalAccessor _currentPrincipalAccessor;
    private readonly IdentityUserManager _userManager;

    public InvitationPageTests()
    {
        _invitationRepository = GetRequiredService<IInvitationRepository>();
        _passwordHasher = GetRequiredService<IPasswordHasher<Invitation>>();
        _invitationAppService = GetRequiredService<IInvitationAppService>();
        _permissionDataSeeder = GetRequiredService<IPermissionDataSeeder>();
        _currentPrincipalAccessor = GetRequiredService<ICurrentPrincipalAccessor>();
        _userManager = GetRequiredService<IdentityUserManager>();
    }

    private async Task AuthenticateAsAdminAsync()
    {
        // Create admin user if it doesn't exist
        var adminUser = await _userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new Volo.Abp.Identity.IdentityUser(Guid.NewGuid(), "admin", "admin@abp.io")
            {
                Name = "admin",
                Surname = "admin"
            };
            await _userManager.CreateAsync(adminUser, "1q2w3E*");
        }

        // Seed required permissions
        await _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            "admin",
            new[] { 
                easyshifthqPermissions.TeamManagement.Default,
                easyshifthqPermissions.TeamManagement.Create,
                easyshifthqPermissions.TeamManagement.BulkInvite 
            }
        );

        // Set the current principal
        var claims = new List<Claim>
        {
            new Claim(AbpClaimTypes.UserId, adminUser.Id.ToString()),
            new Claim(AbpClaimTypes.UserName, adminUser.UserName),
            new Claim(AbpClaimTypes.Email, adminUser.Email)
        };

        (_currentPrincipalAccessor as TestCurrentPrincipalAccessor)?.SetCurrentPrincipal(
            new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"))
        );
    }

    [Fact]
    public async Task Should_Display_Invitations_Page()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        // Act
        var response = await GetResponseAsStringAsync("/Invitations");

        // Assert
        response.ShouldContain("Invitations");
        response.ShouldContain("InvitationsTable");
    }

    // [Fact]
    // public async Task Should_Create_New_Invitation()
    // {
    //     // Arrange
    //     await AuthenticateAsAdminAsync();

    //     // Act
    //     var postData = new Dictionary<string, string>
    //     {
    //         { "Invitation.Email", "newuser@example.com" },
    //         { "Invitation.FirstName", "New" },
    //         { "Invitation.LastName", "User" },
    //         { "Invitation.Role", "employee" }
    //     };

    //     var response = await Client.PostAsync("/Invitations/CreateModal", 
    //         new FormUrlEncodedContent(postData));

    //     // Assert
    //     response.StatusCode.ShouldBe(HttpStatusCode.OK);

    //     // Verify the invitation was created
    //     var invitations = await _invitationAppService.GetPendingAsync();
    //     invitations.ShouldContain(x => x.Email == "newuser@example.com");
    // }

    // [Fact]
    // public async Task Should_Create_Bulk_Invitations()
    // {
    //     // Arrange
    //     await AuthenticateAsAdminAsync();

    //     var testEmails = new[] { "bulk1@example.com", "bulk2@example.com" };
    //     var csvContent = "Email,FirstName,LastName,Role\n" +
    //                     "bulk1@example.com,Bulk,One,employee\n" +
    //                     "bulk2@example.com,Bulk,Two,manager";

    //     var formData = new MultipartFormDataContent();
    //     formData.Add(new StringContent(csvContent), "csvFile", "test.csv");
    //     formData.Add(new StringContent("employee"), "DefaultRole");

    //     // Act
    //     var response = await Client.PostAsync("/Invitations/BulkCreateModal", formData);

    //     // Assert
    //     response.StatusCode.ShouldBe(HttpStatusCode.OK);

    //     // Verify the invitations were created
    //     var invitations = await _invitationAppService.GetPendingAsync();
    //     foreach (var email in testEmails)
    //     {
    //         invitations.ShouldContain(x => x.Email == email);
    //     }
    // }

    [Fact]
    public async Task Should_Accept_Invitation()
    {
        // Arrange
        await AuthenticateAsAdminAsync();

        var invitation = await CreateTestInvitation();
        var token = GenerateValidTokenForInvitation(invitation);

        // Act
        var response = await GetResponseAsync($"/AcceptInvitation?token={token}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain(invitation.Email);
        content.ShouldContain(invitation.FirstName);
    }

    private async Task<InvitationDto> CreateTestInvitation()
    {
        return await _invitationAppService.CreateAsync(new CreateInvitationDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = "employee"
        });
    }

    private string GenerateValidTokenForInvitation(InvitationDto invitationDto)
    {
        var token = Guid.NewGuid().ToString("N");
        
        using (var scope = ServiceProvider.CreateScope())
        {
            var uowManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            using (var uow = uowManager.Begin())
            {
                var invitation = _invitationRepository.GetAsync(invitationDto.Id).GetAwaiter().GetResult();
                var hash = _passwordHasher.HashPassword(invitation, token);
                invitation.SetTokenHash(hash);
                _invitationRepository.UpdateAsync(invitation).GetAwaiter().GetResult();
                uow.CompleteAsync().GetAwaiter().GetResult();
            }
        }

        return token;
    }
}