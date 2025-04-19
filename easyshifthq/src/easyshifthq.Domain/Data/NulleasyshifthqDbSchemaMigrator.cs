using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace easyshifthq.Data;

/* This is used if database provider does't define
 * IeasyshifthqDbSchemaMigrator implementation.
 */
public class NulleasyshifthqDbSchemaMigrator : IeasyshifthqDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
