using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace easyshifthq.Web.Components;

public class BulkInviteModalViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("~/Components/BulkInviteModal.cshtml");
    }
}