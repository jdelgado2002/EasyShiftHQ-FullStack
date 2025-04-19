using Volo.Abp.Modularity;

namespace easyshifthq;

public abstract class easyshifthqApplicationTestBase<TStartupModule> : easyshifthqTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
