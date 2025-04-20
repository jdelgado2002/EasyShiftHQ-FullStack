using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.TenantManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using easyshifthq.Invitations;

namespace easyshifthq;

[DependsOn(
    typeof(easyshifthqDomainModule),
    typeof(easyshifthqApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class easyshifthqApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<easyshifthqApplicationModule>();
        });

        context.Services.AddScoped<IPasswordHasher<Invitation>, PasswordHasher<Invitation>>();
    }
}
