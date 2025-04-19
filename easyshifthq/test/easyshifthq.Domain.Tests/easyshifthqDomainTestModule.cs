using Volo.Abp.Modularity;

namespace easyshifthq;

[DependsOn(
    typeof(easyshifthqDomainModule),
    typeof(easyshifthqTestBaseModule)
)]
public class easyshifthqDomainTestModule : AbpModule
{

}
