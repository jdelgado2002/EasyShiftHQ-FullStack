using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;
using easyshifthq.Invitations;

namespace easyshifthq.Locations;

public class InvitationLocation : Entity, IMultiTenant
{
    public Guid InvitationId { get; private set; }
    public Guid LocationId { get; private set; }
    public Guid? TenantId { get; set; }

    public virtual Invitation Invitation { get; private set; } = null!;
    public virtual Location Location { get; private set; } = null!;

    private InvitationLocation() { } // For EF Core

    public InvitationLocation(Guid invitationId, Guid locationId, Guid? tenantId = null)
    {
        InvitationId = invitationId;
        LocationId = locationId;
        TenantId = tenantId;
    }

    public override object[] GetKeys()
    {
        return new object[] { InvitationId, LocationId };
    }
}