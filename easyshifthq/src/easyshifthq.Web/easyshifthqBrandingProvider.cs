using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Localization;
using easyshifthq.Localization;

namespace easyshifthq.Web;

[Dependency(ReplaceServices = true)]
public class easyshifthqBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<easyshifthqResource> _localizer;

    public easyshifthqBrandingProvider(IStringLocalizer<easyshifthqResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
