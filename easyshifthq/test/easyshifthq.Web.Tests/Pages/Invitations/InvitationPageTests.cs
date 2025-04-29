using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using easyshifthq.Invitations;
using Shouldly;
using Xunit;

namespace easyshifthq.Web.Pages.Invitations;

public class InvitationPageTests : easyshifthqWebTestBase
{
    [Fact]
    public async Task Should_Display_Invitations_Page()
    {
        // Act
        var response = await GetResponseAsStringAsync("/Invitations");

        // Assert
        response.ShouldContain("Invitations");
        response.ShouldContain("InvitationsTable");
    }

    [Fact]
    public async Task Should_Create_New_Invitation()
    {
        // Act
        var postData = new Dictionary<string, string>
        {
            { "Invitation.Email", "newuser@example.com" },
            { "Invitation.FirstName", "New" },
            { "Invitation.LastName", "User" },
            { "Invitation.Role", "employee" }
        };

        var response = await Client.PostAsync("/Invitations/CreateModal", 
            new FormUrlEncodedContent(postData));

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the invitation was created
        var invitationService = GetRequiredService<IInvitationAppService>();
        var invitations = await invitationService.GetPendingAsync();
        invitations.ShouldContain(x => x.Email == "newuser@example.com");
    }

    [Fact]
    public async Task Should_Create_Bulk_Invitations()
    {
        // Arrange
        var testEmails = new[] { "bulk1@example.com", "bulk2@example.com" };
        var csvContent = "Email,FirstName,LastName,Role\n" +
                        "bulk1@example.com,Bulk,One,employee\n" +
                        "bulk2@example.com,Bulk,Two,manager";

        var formData = new MultipartFormDataContent();
        formData.Add(new StringContent(csvContent), "csvFile", "test.csv");
        formData.Add(new StringContent("employee"), "DefaultRole");

        // Act
        var response = await Client.PostAsync("/Invitations/BulkCreateModal", formData);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the invitations were created
        var invitationService = GetRequiredService<IInvitationAppService>();
        var invitations = await invitationService.GetPendingAsync();
        foreach (var email in testEmails)
        {
            invitations.ShouldContain(x => x.Email == email);
        }
    }

    [Fact]
    public async Task Should_Accept_Invitation()
    {
        // Arrange
        var invitationService = GetRequiredService<IInvitationAppService>();
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
        var invitationService = GetRequiredService<IInvitationAppService>();
        return await invitationService.CreateAsync(new CreateInvitationDto
        {
            Email = $"test{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "User",
            Role = "employee"
        });
    }

    private string GenerateValidTokenForInvitation(InvitationDto invitation)
    {
        // In a real test, you'd need to get this from the email service mock
        // or implement a test-specific way to get a valid token
        return Guid.NewGuid().ToString("N");
    }
}