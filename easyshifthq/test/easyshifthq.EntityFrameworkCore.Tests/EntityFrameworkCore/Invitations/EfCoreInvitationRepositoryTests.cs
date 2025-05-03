using System;
using System.Threading.Tasks;
using easyshifthq.Invitations;
using Shouldly;
using Xunit;

namespace easyshifthq.EntityFrameworkCore.Invitations;

[Collection(easyshifthqTestConsts.CollectionDefinitionName)]
public class EfCoreInvitationRepositoryTests : easyshifthqEntityFrameworkCoreTestBase
{
    private readonly IInvitationRepository _invitationRepository;

    public EfCoreInvitationRepositoryTests()
    {
        _invitationRepository = GetRequiredService<IInvitationRepository>();
    }

    [Fact]
    public async Task Should_Find_Invitation_By_Email()
    {
        // Arrange
        var invitation = await CreateAndSaveTestInvitation();

        // Act
        var found = await _invitationRepository.FindByEmailAsync(invitation.Email);

        // Assert
        found.ShouldNotBeNull();
        found.Id.ShouldBe(invitation.Id);
    }

    [Fact]
    public async Task Should_Find_Invitation_By_Token_Hash()
    {
        // Arrange
        var invitation = await CreateAndSaveTestInvitation();

        // Act
        var found = await _invitationRepository.FindByTokenHashAsync(invitation.TokenHash);

        // Assert
        found.ShouldNotBeNull();
        found.Id.ShouldBe(invitation.Id);
    }

    [Fact]
    public async Task Should_Get_Pending_Invitations()
    {
        // Arrange
        var pendingInvitation = await CreateAndSaveTestInvitation();
        var acceptedInvitation = await CreateAndSaveTestInvitation();
        await WithUnitOfWorkAsync(async () =>
        {
            acceptedInvitation.Accept();
            await _invitationRepository.UpdateAsync(acceptedInvitation);
        });

        // Act
        var pendingInvitations = await _invitationRepository.GetPendingInvitationsAsync();

        // Assert
        pendingInvitations.ShouldContain(x => x.Id == pendingInvitation.Id);
        pendingInvitations.ShouldNotContain(x => x.Id == acceptedInvitation.Id);
    }

    [Fact]
    public async Task Should_Check_If_Email_Exists()
    {
        // Arrange
        var invitation = await CreateAndSaveTestInvitation();

        // Act
        var exists = await _invitationRepository.EmailExistsAsync(invitation.Email);
        var notExists = await _invitationRepository.EmailExistsAsync("nonexistent@example.com");

        // Assert
        exists.ShouldBeTrue();
        notExists.ShouldBeFalse();
    }

    private async Task<Invitation> CreateAndSaveTestInvitation()
    {
        var invitation = new Invitation(
            Guid.NewGuid(),
            "test@example.com",
            "John",
            "Doe",
            "employee",
            Guid.NewGuid(),
            "test-token-hash"
        );

        await WithUnitOfWorkAsync(async () =>
        {
            await _invitationRepository.InsertAsync(invitation);
        });

        return invitation;
    }
}