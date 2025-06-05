using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using easyshifthq.Locations;
using Volo.Abp;

namespace easyshifthq.Invitations;

public class Invitation : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Role { get; private set; }
    public string TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public InvitationStatus Status { get; private set; }
    public Guid? TenantId { get; set; }
    public ICollection<InvitationLocation> InvitationLocations { get; private set; }

    private Invitation() 
    { 
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        Role = string.Empty;
        TokenHash = string.Empty;
        InvitationLocations = new List<InvitationLocation>();
    }

    public Invitation(
        Guid id,
        string email,
        string firstName,
        string lastName,
        string role,
        string tokenHash,
        Guid? tenantId = null)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        TokenHash = tokenHash;
        Status = InvitationStatus.Pending;
        ExpiresAt = DateTime.UtcNow.AddDays(7);
        TenantId = tenantId;
        InvitationLocations = new List<InvitationLocation>();
    }

    public void Accept()
    {
        // Validate current state
        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Invitation has expired.");
        }

        if (IsExpired())
        {
            throw new InvalidOperationException("Invitation has already been accepted or revoked.");
        }

        // Update status
        Status = InvitationStatus.Accepted;
    }

    public void Revoke()
    {
        if (Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Invitation has already been accepted or revoked.");
        }
        if (IsExpired())
        {
            throw new InvalidOperationException("Invitation has expired.");
        }
        // If the invitation is accepted, we can still revoke it
        Status = InvitationStatus.Revoked;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    public void SetTokenHash(string tokenHash)
    {
        if (string.IsNullOrEmpty(tokenHash))
        {
            throw new ArgumentException("Token hash cannot be null or empty.", nameof(tokenHash));
        }
        if (tokenHash.Length > 128)
        {
            throw new ArgumentException("Token hash cannot exceed 128 characters.", nameof(tokenHash));
        }
        TokenHash = tokenHash;
    }

    public void SetExpiresAt(DateTime expiresAt)
    {
        if (expiresAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Expiration date must be in the future.", nameof(expiresAt));
        }
        if (expiresAt > DateTime.UtcNow.AddDays(30))
        {
            throw new ArgumentException("Expiration date cannot exceed 30 days from now.", nameof(expiresAt));
        }
        if (expiresAt.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Expiration date must be in UTC.", nameof(expiresAt));
        }
        ExpiresAt = expiresAt;
    }

    public void SetStatus(InvitationStatus status)
    {
        if (status == InvitationStatus.Accepted && Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Cannot accept an invitation that is not pending.");
        }
        if (status == InvitationStatus.Revoked && Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException("Cannot revoke an invitation that is not pending.");
        }
        Status = status;
    }

    public void AddLocation(Guid locationId)
    {
        if (locationId == Guid.Empty)
        {
            throw new ArgumentException("Location ID cannot be an empty GUID.", nameof(locationId));
        }

        if (InvitationLocations.Any(x => x.LocationId == locationId))
        {
            return;
        }

        InvitationLocations.Add(new InvitationLocation(Id, locationId, TenantId));
    }

    public void RemoveLocation(Guid locationId)
    {
        var location = InvitationLocations.FirstOrDefault(x => x.LocationId == locationId);
        if (location != null)
        {
            InvitationLocations.Remove(location);
        }
    }
}
