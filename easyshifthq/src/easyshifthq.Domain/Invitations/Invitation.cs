using System;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace easyshifthq.Invitations;

public class Invitation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Role { get; private set; }
    public Guid? LocationId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public InvitationStatus Status { get; private set; }
    public Guid? TenantId { get; set; }

    private Invitation() { } // For EF Core

    public Invitation(
        Guid id,
        string email,
        string firstName,
        string lastName,
        string role,
        Guid? locationId,
        string tokenHash,
        Guid? tenantId = null)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        LocationId = locationId;
        TokenHash = tokenHash;
        Status = InvitationStatus.Pending;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
        TenantId = tenantId;
    }

    public void Accept()
    {
        Status = InvitationStatus.Accepted;
    }

    public void Revoke()
    {
        Status = InvitationStatus.Revoked;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }
}