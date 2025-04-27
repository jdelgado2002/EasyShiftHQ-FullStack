using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Guids;

namespace easyshifthq.Data;

public class IdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    public const string AdminEmailPropertyName = "AdminEmail";
    public const string AdminPasswordPropertyName = "AdminPassword";

    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IdentityRoleManager _roleManager;

    public IdentityDataSeedContributor(
        IIdentityRoleRepository roleRepository,
        IGuidGenerator guidGenerator,
        IdentityRoleManager roleManager)
    {
        _roleRepository = roleRepository;
        _guidGenerator = guidGenerator;
        _roleManager = roleManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateRoleIfNotExistsAsync("Admin");
        await CreateRoleIfNotExistsAsync("Manager");
        await CreateRoleIfNotExistsAsync("Employee");
    }

    private async Task CreateRoleIfNotExistsAsync(string roleName)
    {
        if (await _roleRepository.FindByNormalizedNameAsync(roleName.ToUpperInvariant()) == null)
        {
            var role = new IdentityRole(_guidGenerator.Create(), roleName)
            {
                IsStatic = true,
                IsPublic = true
            };

            await _roleManager.CreateAsync(role);
        }
    }
}