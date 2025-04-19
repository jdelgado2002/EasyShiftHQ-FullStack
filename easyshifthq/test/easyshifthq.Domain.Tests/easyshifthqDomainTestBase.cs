using Volo.Abp.Modularity;

namespace easyshifthq;

/* Inherit from this class for your domain layer tests. */
public abstract class easyshifthqDomainTestBase<TStartupModule> : easyshifthqTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
