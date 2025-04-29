using System;
using Shouldly;
using Xunit;

namespace easyshifthq.Invitations;

public class InvitationTests : easyshifthqDomainTestBase<easyshifthqDomainTestModule>
{
    [Fact]
    public void Should_Create_Valid_Invitation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var role = "employee";
        var locationId = Guid.NewGuid();
        var tokenHash = "test-token-hash";

        // Act
        var invitation = new Invitation(
            id,
            email,
            firstName,
            lastName,
            role,
            locationId,
            tokenHash
        );

        // Assert
        invitation.Id.ShouldBe(id);
        invitation.Email.ShouldBe(email);
        invitation.FirstName.ShouldBe(firstName);
        invitation.LastName.ShouldBe(lastName);
        invitation.Role.ShouldBe(role);
        invitation.LocationId.ShouldBe(locationId);
        invitation.TokenHash.ShouldBe(tokenHash);
        invitation.Status.ShouldBe(InvitationStatus.Pending);
        invitation.ExpiresAt.ShouldBeGreaterThan(DateTime.UtcNow);
    }

    [Fact]
    public void Should_Accept_Valid_Pending_Invitation()
    {
        // Arrange
        var invitation = CreateValidInvitation();

        // Act
        invitation.Accept();

        // Assert
        invitation.Status.ShouldBe(InvitationStatus.Accepted);
    }

    [Fact]
    public void Should_Not_Accept_Already_Accepted_Invitation()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        invitation.Accept();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            invitation.Accept();
        });
    }

    [Fact]
    public void Should_Not_Accept_Revoked_Invitation()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        invitation.Revoke();

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            invitation.Accept();
        });
    }

    [Fact]
    public void Should_Not_Accept_Expired_Invitation()
    {
        // Arrange
        var invitation = CreateValidInvitation();
        invitation.SetExpiresAt(DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        Should.Throw<InvalidOperationException>(() =>
        {
            invitation.Accept();
        });
    }

    [Fact]
    public void Should_Revoke_Valid_Pending_Invitation()
    {
        // Arrange
        var invitation = CreateValidInvitation();

        // Act
        invitation.Revoke();

        // Assert
        invitation.Status.ShouldBe(InvitationStatus.Revoked);
    }

    [Fact]
    public void Should_Not_Set_Invalid_Token_Hash()
    {
        // Arrange
        var invitation = CreateValidInvitation();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
        {
            invitation.SetTokenHash("");
        });

        Should.Throw<ArgumentException>(() =>
        {
            invitation.SetTokenHash(new string('x', 129)); // > 128 chars
        });
    }

    [Fact]
    public void Should_Not_Set_Invalid_Expiration_Date()
    {
        // Arrange
        var invitation = CreateValidInvitation();

        // Act & Assert
        Should.Throw<ArgumentException>(() =>
        {
            invitation.SetExpiresAt(DateTime.UtcNow.AddDays(-1));
        });

        Should.Throw<ArgumentException>(() =>
        {
            invitation.SetExpiresAt(DateTime.UtcNow.AddDays(31));
        });

        Should.Throw<ArgumentException>(() =>
        {
            invitation.SetExpiresAt(DateTime.Now); // Non-UTC
        });
    }

    private static Invitation CreateValidInvitation()
    {
        return new Invitation(
            Guid.NewGuid(),
            "test@example.com",
            "John",
            "Doe",
            "Employee",
            null,
            string.Empty,
            null
        );
    }
}