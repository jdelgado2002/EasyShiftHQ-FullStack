using easyshifthq.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace easyshifthq.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class easyshifthqController : AbpControllerBase
{
    protected easyshifthqController()
    {
        LocalizationResource = typeof(easyshifthqResource);
    }
}
