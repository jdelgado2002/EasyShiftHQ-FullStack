using System;
using System.ComponentModel.DataAnnotations;

namespace easyshifthq.Invitations;

public class CreateInvitationDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [StringLength(64)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(64)]
    public string LastName { get; set; }

    [Required]
    public string Role { get; set; }

    public Guid? LocationId { get; set; }
}