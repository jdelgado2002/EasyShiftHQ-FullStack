using easyshifthq.Localization;
using Volo.Abp.Application.Services;

namespace easyshifthq;

/* Inherit your application services from this class.
 */
public abstract class easyshifthqAppService : ApplicationService
{
    protected easyshifthqAppService()
    {
        LocalizationResource = typeof(easyshifthqResource);
    }
}
