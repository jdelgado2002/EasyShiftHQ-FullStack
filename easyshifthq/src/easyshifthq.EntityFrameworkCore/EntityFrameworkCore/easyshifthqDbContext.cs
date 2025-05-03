using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using easyshifthq.Invitations;
using easyshifthq.Locations;

namespace easyshifthq.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class EasyshifthqDbContext :
    AbpDbContext<EasyshifthqDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<InvitationLocation> InvitationLocations { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext and ISaasDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext and ISaasDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public EasyshifthqDbContext(DbContextOptions<EasyshifthqDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        /* Include modules to your migration db context */

        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureBackgroundJobs();
        modelBuilder.ConfigureAuditLogging();
        modelBuilder.ConfigureFeatureManagement();
        modelBuilder.ConfigureIdentity();
        modelBuilder.ConfigureOpenIddict();
        modelBuilder.ConfigureTenantManagement();
        modelBuilder.ConfigureBlobStoring();
        
        /* Configure your own tables/entities inside here */

        modelBuilder.Entity<Invitation>(b =>
        {
            b.ToTable(easyshifthqConsts.DbTablePrefix + "Invitations", easyshifthqConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.FirstName).IsRequired().HasMaxLength(64);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(64);
            b.Property(x => x.Role).IsRequired().HasMaxLength(64);
            b.Property(x => x.TokenHash).IsRequired();
            
            b.HasIndex(x => x.Email);

            b.HasMany(x => x.InvitationLocations)
             .WithOne(x => x.Invitation)
             .HasForeignKey(x => x.InvitationId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Location>(b =>
        {
            b.ToTable(easyshifthqConsts.DbTablePrefix + "Locations", easyshifthqConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
            b.Property(x => x.Address).IsRequired();
            b.Property(x => x.TimeZone).IsRequired();
            b.Property(x => x.JurisdictionCode).HasMaxLength(50);
            b.Property(x => x.Notes).HasMaxLength(500);
            
            b.HasIndex(x => x.Name);
            b.HasIndex(x => x.TimeZone);
            b.HasIndex(x => x.JurisdictionCode);

            b.HasMany<InvitationLocation>()
             .WithOne(x => x.Location)
             .HasForeignKey(x => x.LocationId)
             .IsRequired()
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InvitationLocation>(b =>
        {
            b.ToTable(easyshifthqConsts.DbTablePrefix + "InvitationLocations", easyshifthqConsts.DbSchema);
            b.ConfigureByConvention();
            
            b.HasKey(x => new { x.InvitationId, x.LocationId });
        });
    }
}
