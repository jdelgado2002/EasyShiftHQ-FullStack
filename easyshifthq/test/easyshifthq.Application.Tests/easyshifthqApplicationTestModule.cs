using Volo.Abp.Modularity;

namespace easyshifthq;

[DependsOn(
    typeof(easyshifthqApplicationModule),
    typeof(easyshifthqDomainTestModule)
)]
public class easyshifthqApplicationTestModule : AbpModule
{

}
