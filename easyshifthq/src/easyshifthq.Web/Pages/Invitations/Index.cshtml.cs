using Microsoft.AspNetCore.Authorization;
using easyshifthq.Invitations;

namespace easyshifthq.Web.Pages.Invitations;

[Authorize]
public class IndexModel : easyshifthqPageModel
{
    private readonly IInvitationAppService _invitationAppService;

    public IndexModel(IInvitationAppService invitationAppService)
    {
        _invitationAppService = invitationAppService;
    }

    public void OnGet()
    {
    }
}