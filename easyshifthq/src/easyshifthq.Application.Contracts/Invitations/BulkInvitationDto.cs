using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Invitations;

public class BulkInvitationDto
{
    [Required]
    public List<CreateInvitationDto> Invitations { get; set; }
}