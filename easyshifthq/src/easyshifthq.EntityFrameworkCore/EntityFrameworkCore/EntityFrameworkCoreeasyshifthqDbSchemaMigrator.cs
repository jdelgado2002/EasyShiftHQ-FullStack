using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using easyshifthq.Data;
using Volo.Abp.DependencyInjection;

namespace easyshifthq.EntityFrameworkCore;

public class EntityFrameworkCoreeasyshifthqDbSchemaMigrator
    : IeasyshifthqDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreeasyshifthqDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the easyshifthqDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<easyshifthqDbContext>()
            .Database
            .MigrateAsync();
    }
}
