using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace easyshifthq.Security;

[Dependency(ReplaceServices = true)]
public class TestCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    private ClaimsPrincipal _principal;

    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return _principal ?? base.GetClaimsPrincipal();
    }

    public virtual void SetCurrentPrincipal(ClaimsPrincipal principal)
    {
        _principal = principal;
    }
}