using easyshifthq.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace easyshifthq.Web.Pages;

public abstract class easyshifthqPageModel : AbpPageModel
{
    protected easyshifthqPageModel()
    {
        LocalizationResourceType = typeof(easyshifthqResource);
    }
}
