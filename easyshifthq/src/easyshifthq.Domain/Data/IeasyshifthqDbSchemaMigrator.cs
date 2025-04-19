using System.Threading.Tasks;

namespace easyshifthq.Data;

public interface IeasyshifthqDbSchemaMigrator
{
    Task MigrateAsync();
}
