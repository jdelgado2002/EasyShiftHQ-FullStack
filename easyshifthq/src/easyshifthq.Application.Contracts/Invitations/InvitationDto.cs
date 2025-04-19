using System;
using Volo.Abp.Application.Dtos;

namespace easyshifthq.Invitations;

public class InvitationDto : AuditedEntityDto<Guid>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public Guid? LocationId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public InvitationStatus Status { get; set; }
}