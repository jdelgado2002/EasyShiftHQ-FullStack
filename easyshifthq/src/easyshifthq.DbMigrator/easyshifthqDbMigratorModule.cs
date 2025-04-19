using easyshifthq.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace easyshifthq.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(easyshifthqEntityFrameworkCoreModule),
    typeof(easyshifthqApplicationContractsModule)
)]
public class easyshifthqDbMigratorModule : AbpModule
{
}
