using easyshifthq.Invitations;
using Xunit;

namespace easyshifthq.EntityFrameworkCore.Invitations
{
    [Collection(easyshifthqTestConsts.CollectionDefinitionName)]
    public class EfCoreInvitationAppService_Tests : InvitationAppServiceTests<easyshifthqEntityFrameworkCoreTestModule>
    {

    }
}