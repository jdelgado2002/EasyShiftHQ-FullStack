# EasyShiftHQ Development Guidelines

## 1. Project Structure
- Solution is organized into multiple projects following ABP.io's layered architecture:
  - `easyshifthq.Domain`: Core domain logic and entities
  - `easyshifthq.EntityFrameworkCore`: Data access layer with EF Core
  - `easyshifthq.HttpApi`: API endpoints and DTOs
  - `easyshifthq.Web`: MVC/Razor Pages UI
  - `easyshifthq.DbMigrator`: Database migration tool
  - `easyshifthq.Application`: Application services and business logic
  - `easyshifthq.Application.Contracts`: Interfaces and DTOs for application services

## 2. ABP.io Framework Features
- Utilize built-in ABP.io features:
  - Dependency Injection (Autofac)
  - Authorization and Permission System
  - Multi-tenancy Support
  - Localization System
  - Background Jobs
  - Distributed Event Bus
  - Audit Logging
  - Setting Management
  - Feature Management

### Module Configuration Example:
````csharp
// filepath: easyshifthq.Web/easyshifthqWebModule.cs
[DependsOn(
    typeof(easyshifthqApplicationModule),
    typeof(easyshifthqHttpApiModule),
    typeof(AbpAspNetCoreMvcUiThemeLeptonXLiteModule)
)]
public class easyshifthqWebModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new easyshifthqMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<easyshifthqWebModule>();
        });
    }
}
````

## 3. UI Development with ABP.io
### Theme System
- ABP.io provides two free pre-built UI themes:
  - Basic theme: Minimalist theme built on plain Bootstrap styling
  - LeptonX theme: Modern, production-ready UI theme
- Themes determine base libraries and standards:
  - Bootstrap 5.x
  - Datatables.Net
  - JQuery and JQuery Validation
  - FontAwesome
  - Sweetalert
  - Toastr
  - Lodash

### Layout System
- ABP.io defines four standard layouts:
  - Application: Default layout for back-office applications
  - Account: For login, register, and account-related pages
  - Public: For public-facing websites
  - Empty: Layout without actual layout structure

### Layout Configuration Example:
````csharp
// filepath: easyshifthq.Web/Pages/MyPage.cshtml
@inject IThemeManager ThemeManager

@{
    Layout = ThemeManager.CurrentTheme.GetLayout(StandardLayouts.Account);
}
````

### Bundling and Minification
- Use ABP's bundling system for client-side resources:
  - Install NPM packages using `package.json`
  - Configure resource mapping in `abp.resourcemapping.js`
  - Use `abp install-libs` to copy resources to `wwwroot`

### Resource Mapping Example:
````javascript
// filepath: easyshifthq.Web/abp.resourcemapping.js
module.exports = {
    aliases: { },
    mappings: {
        "@node_modules/vue/dist/vue.min.js": "@libs/vue/"
    }
};
````

### Script and Style Management
- Use ABP's tag helpers for scripts and styles:
  - `abp-script` for JavaScript files
  - `abp-style` for CSS files
  - `abp-script-bundle` for bundling multiple scripts
  - `abp-style-bundle` for bundling multiple styles

### Script Bundle Example:
````csharp
@section scripts {
    <abp-script-bundle>
        <abp-script src="/libs/vue/vue.min.js" />
        <abp-script src="/Pages/MyPage.cshtml.js" />
    </abp-script-bundle>
}
````

### Menu System
- Use ABP's menu system for navigation:
  - Define menu items in `IMenuContributor`
  - Use `StandardMenus.Main` for main menu
  - Use `StandardMenus.User` for user context menu

### Menu Configuration Example:
````csharp
// filepath: easyshifthq.Web/Menus/easyshifthqMenuContributor.cs
public class easyshifthqMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            var l = context.GetLocalizer<easyshifthqResource>();
            
            context.Menu.AddItem(
                new ApplicationMenuItem("MyProject.Crm", l["CRM"])
                    .AddItem(new ApplicationMenuItem(
                        name: "MyProject.Crm.Customers",
                        displayName: l["Customers"],
                        url: "/crm/customers")
                    )
            );
        }
    }
}
````

### Bootstrap Tag Helpers
- Use ABP's Bootstrap tag helpers for UI components:
  - `abp-button` for buttons
  - `abp-card` for cards
  - `abp-table` for tables
  - `abp-modal` for modals
  - `abp-input` for form inputs
  - `abp-select` for dropdowns
  - `abp-radio` for radio buttons

### Form Handling
- Use ABP's form tag helpers:
  - `abp-dynamic-form` for automatic form generation
  - `abp-input` for input fields
  - `abp-select` for dropdowns
  - `abp-radio` for radio buttons

### Form Example:
````csharp
// filepath: easyshifthq.Web/Pages/Employees/Create.cshtml
@page
@model CreateModel

<abp-dynamic-form abp-model="Employee" submit-button="true">
    <abp-form-content />
</abp-dynamic-form>
````

### Modal Dialogs
- Use ABP's modal system:
  - Create separate Razor Pages for modals
  - Use `abp-modal` tag helper
  - Use `ModalManager` for client-side control

### Modal Example:
````csharp
// filepath: easyshifthq.Web/Pages/Employees/EditModal.cshtml
@page
@model EditModalModel
@{
    Layout = null;
}

<abp-modal>
    <abp-modal-header title="Edit Employee"></abp-modal-header>
    <abp-modal-body>
        <abp-dynamic-form abp-model="Employee" submit-button="true">
            <abp-form-content />
        </abp-dynamic-form>
    </abp-modal-body>
    <abp-modal-footer buttons="@(AbpModalButtons.Cancel|AbpModalButtons.Save)">
    </abp-modal-footer>
</abp-modal>
````

### JavaScript API
- Use ABP's JavaScript APIs:
  - `abp.currentUser` for user information
  - `abp.auth` for permission checks
  - `abp.features` for feature checks
  - `abp.localization` for localization
  - `abp.message` for message boxes
  - `abp.notify` for notifications
  - `abp.ajax` for API calls

### JavaScript API Example:
````javascript
// filepath: easyshifthq.Web/Pages/Employees/Index.cshtml.js
$(function () {
    if (abp.auth.isGranted('EasyShiftHQ.Employees.Create')) {
        $('#CreateButton').show();
    }
    
    abp.ajax({
        type: 'GET',
        url: '/api/app/employee',
        success: function(result) {
            // Handle result
        }
    });
});
````

### HTTP API Consumption
- Use ABP's API consumption methods:
  - Dynamic JavaScript proxies
  - Static JavaScript proxies
  - `abp.ajax` API

### Dynamic Proxy Example:
````javascript
// Using dynamic JavaScript proxies
easyshifthq.employees.employee
    .get('1b8517c8-2c08-5016-bca8-39fef5c4f817')
    .then(function (result) {
        console.log(result);
    });
````

## 4. Database Management
- Use Entity Framework Core for data access
- Implement migrations in `easyshifthq.EntityFrameworkCore`
- Use the DbMigrator project for database updates
- Follow Code-First approach for schema changes

### Entity Example:
````csharp
// filepath: easyshifthq.Domain/Entities/Employee.cs
public class Employee : FullAuditedAggregateRoot<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    
    private Employee() { } // For EF Core

    public Employee(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}
````

### DbContext Example:
````csharp
// filepath: easyshifthq.EntityFrameworkCore/EntityFrameworkCore/EasyShiftHQDbContext.cs
public class EasyShiftHQDbContext : AbpDbContext<EasyShiftHQDbContext>
{
    public DbSet<Employee> Employees { get; set; }
    
    public EasyShiftHQDbContext(DbContextOptions<EasyShiftHQDbContext> options)
        : base(options)
    {
    }
}
````

### Domain Tests Example:
````csharp
// filepath: easyshifthq.Domain.Tests/Employees/EmployeeTests.cs
public class EmployeeTests : EasyShiftHQDomainTestBase
{
    [Fact]
    public void Should_Create_Valid_Employee()
    {
        var employee = new Employee(
            Guid.NewGuid(),
            "John",
            "Doe",
            "john.doe@example.com"
        );

        employee.FirstName.ShouldBe("John");
        employee.LastName.ShouldBe("Doe");
    }
}
````

### API Controller Example:
````csharp
// filepath: easyshifthq.HttpApi/Controllers/EmployeesController.cs
[Area("app")]
[Route("api/v1/employees")]
public class EmployeesController : EasyShiftHQController
{
    private readonly IEmployeeAppService _employeeService;

    public EmployeesController(IEmployeeAppService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<PagedResultDto<EmployeeDto>> GetListAsync(GetEmployeesInput input)
    {
        return await _employeeService.GetListAsync(input);
    }
}
````

### DTO Example:
````csharp
// filepath: easyshifthq.Application.Contracts/Employees/EmployeeDto.cs
public class EmployeeDto : AuditedEntityDto<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
````

### Frontend Component Example:
````csharp
// filepath: easyshifthq.Web/Pages/Employees/Index.cshtml
@page
@using easyshifthq.Web.Pages.Employees
@model IndexModel
@{
    ViewData["Title"] = "Employees";
}

@section scripts {
    <abp-script src="/Pages/Employees/Index.js" />
}

<abp-card>
    <abp-card-header>
        <h2>@L["Employees"]</h2>
    </abp-card-header>
    <abp-card-body>
        <abp-table striped-rows="true" id="EmployeesTable">
            <thead>
                <tr>
                    <th>@L["FirstName"]</th>
                    <th>@L["LastName"]</th>
                    <th>@L["Email"]</th>
                    <th>@L["Actions"]</th>
                </tr>
            </thead>
        </abp-table>
    </abp-card-body>
</abp-card>

// filepath: easyshifthq.Web/Pages/Employees/Index.cshtml.cs
public class IndexModel : EasyShiftHQPageModel
{
    private readonly IEmployeeAppService _employeeAppService;

    public IndexModel(IEmployeeAppService employeeAppService)
    {
        _employeeAppService = employeeAppService;
    }

    public async Task OnGetAsync()
    {
        // Page initialization logic
    }
}

// filepath: easyshifthq.Web/Pages/Employees/Index.js
$(function () {
    var l = abp.localization.getResource('EasyShiftHQ');
    var dataTable = $('#EmployeesTable').DataTable(
        abp.libs.datatables.normalizeConfiguration({
            serverSide: true,
            paging: true,
            order: [[1, "asc"]],
            searching: false,
            scrollX: true,
            ajax: abp.libs.datatables.createAjax(_employeeAppService.getList),
            columnDefs: [
                {
                    title: l('FirstName'),
                    data: "firstName"
                },
                {
                    title: l('LastName'),
                    data: "lastName"
                },
                {
                    title: l('Email'),
                    data: "email"
                },
                {
                    title: l('Actions'),
                    rowAction: {
                        items:
                            [
                                {
                                    text: l('Edit'),
                                    visible: abp.auth.isGranted('EasyShiftHQ.Employees.Edit'),
                                    action: function (data) {
                                        location.href = '/Employees/Edit/' + data.record.id;
                                    }
                                },
                                {
                                    text: l('Delete'),
                                    visible: abp.auth.isGranted('EasyShiftHQ.Employees.Delete'),
                                    action: function (data) {
                                        abp.message.confirm(
                                            l('EmployeeDeletionConfirmationMessage'),
                                            null,
                                            function (isConfirmed) {
                                                if (isConfirmed) {
                                                    _employeeAppService
                                                        .delete(data.record.id)
                                                        .then(function () {
                                                            abp.notify.info(l('SuccessfullyDeleted'));
                                                            dataTable.ajax.reload();
                                                        });
                                                }
                                            }
                                        );
                                    }
                                }
                            ]
                    }
                }
            ]
        })
    );
});
````

## 5. Frontend Development
- Use ABP.io's built-in UI components and themes
- Follow ABP.io's page organization:
  - Place pages in `Pages` directory
  - Use `@page` directive for routing
  - Implement page models in `.cshtml.cs` files
  - Use ABP's localization system with `@L` helper
  - Leverage ABP's built-in UI components:
    - `abp-card`
    - `abp-table`
    - `abp-button`
    - `abp-modal`
    - `abp-dynamic-form`

### Page Model Example:
````csharp
// filepath: easyshifthq.Web/Pages/Employees/Create.cshtml.cs
public class CreateModel : EasyShiftHQPageModel
{
    [BindProperty]
    public CreateUpdateEmployeeDto Employee { get; set; }

    private readonly IEmployeeAppService _employeeAppService;

    public CreateModel(IEmployeeAppService employeeAppService)
    {
        _employeeAppService = employeeAppService;
    }

    public void OnGet()
    {
        Employee = new CreateUpdateEmployeeDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _employeeAppService.CreateAsync(Employee);
        return NoContent();
    }
}
````

### Dynamic Form Example:
````csharp
// filepath: easyshifthq.Web/Pages/Employees/Create.cshtml
@page
@model CreateModel

<abp-dynamic-form abp-model="Employee" submit-button="true">
    <abp-form-content />
</abp-dynamic-form>
````

## 6. Authorization and Permissions
- Use ABP's permission system:
  - Define permissions in `PermissionDefinitionProvider`
  - Apply permissions using `[Authorize]` attribute
  - Check permissions in UI with `abp.auth.isGranted()`

### Permission Definition Example:
````csharp
// filepath: easyshifthq.Application/Employees/EmployeePermissionDefinitionProvider.cs
public class EmployeePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var employeeGroup = context.AddGroup(EmployeePermissions.GroupName);
        
        employeeGroup.AddPermission(EmployeePermissions.Employees.Default);
        employeeGroup.AddPermission(EmployeePermissions.Employees.Create);
        employeeGroup.AddPermission(EmployeePermissions.Employees.Edit);
        employeeGroup.AddPermission(EmployeePermissions.Employees.Delete);
    }
}
````

## 7. Localization
- Use ABP's localization system:
  - Define strings in JSON files
  - Use `@L["Key"]` in views
  - Use `L["Key"]` in C# code
  - Use `abp.localization.getResource()` in JavaScript

### Localization Example:
````json
// filepath: easyshifthq.Domain.Shared/Localization/en.json
{
  "culture": "en",
  "texts": {
    "Menu:Employees": "Employees",
    "Employees": "Employees",
    "FirstName": "First Name",
    "LastName": "Last Name",
    "Email": "Email"
  }
}
````

## 8. Background Jobs
- Use ABP's background job system:
  - Implement `IBackgroundJob<TArgs>`
  - Use `IBackgroundJobManager` to enqueue jobs
  - Configure job options in module

### Background Job Example:
````csharp
// filepath: easyshifthq.Application/BackgroundJobs/EmployeeSyncJob.cs
public class EmployeeSyncJob : AsyncBackgroundJob<EmployeeSyncArgs>, ITransientDependency
{
    private readonly IEmployeeAppService _employeeAppService;

    public EmployeeSyncJob(IEmployeeAppService employeeAppService)
    {
        _employeeAppService = employeeAppService;
    }

    public override async Task ExecuteAsync(EmployeeSyncArgs args)
    {
        await _employeeAppService.SyncEmployeesAsync(args);
    }
}
````

## 9. Settings Management
- Use ABP's setting system:
  - Define settings in `SettingDefinitionProvider`
  - Access settings using `ISettingProvider`
  - Use settings in UI with `abp.setting.get()`

### Setting Definition Example:
````csharp
// filepath: easyshifthq.Application/Settings/EmployeeSettingDefinitionProvider.cs
public class EmployeeSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                EmployeeSettings.MaxEmployeeCount,
                "100",
                L("DisplayName:MaxEmployeeCount"),
                L("Description:MaxEmployeeCount")
            )
        );
    }
}
````

## 10. Audit Logging
- Use ABP's audit logging:
  - Apply `[Audited]` attribute to entities
  - Use `IAuditingManager` for custom audit logs
  - Configure audit options in module

### Audit Logging Example:
````csharp
// filepath: easyshifthq.Domain/Entities/Employee.cs
[Audited]
public class Employee : FullAuditedAggregateRoot<Guid>
{
    // ... existing code ...
}
````

## 11. Testing
- Follow ABP's testing patterns:
  - Use `AbpIntegratedTest<TStartupModule>` for integration tests
  - Use `AbpApplicationFactory<TStartupModule>` for web tests
  - Mock services using ABP's test infrastructure

### Integration Test Example:
````csharp
// filepath: easyshifthq.Application.Tests/Employees/EmployeeAppService_Tests.cs
public class EmployeeAppService_Tests : easyshifthqApplicationTestBase
{
    private readonly IEmployeeAppService _employeeAppService;

    public EmployeeAppService_Tests()
    {
        _employeeAppService = GetRequiredService<IEmployeeAppService>();
    }

    [Fact]
    public async Task Should_Create_Employee()
    {
        var result = await _employeeAppService.CreateAsync(
            new CreateUpdateEmployeeDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            }
        );

        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("John");
    }
}
````

## 12. Testing Organization
- Maintain separate test projects:
  - `easyshifthq.EntityFrameworkCore.Tests`
  - `easyshifthq.Web.Tests`
- Write integration tests for data access
- Include UI component tests

### Test Structure Pattern
When creating tests that need to store or access data:

1. Create an abstract base test class with the module type parameter:
```csharp
public abstract class MyFeatureTests<TStartupModule> : easyshifthqApplicationTestBase 
    where TStartupModule : IAbpModule
{
    private readonly IMyService _myService;

    protected MyFeatureTests()
    {
        _myService = GetRequiredService<IMyService>();
    }

    [Fact]
    public async Task Should_Do_Something()
    {
        // Your test implementation
    }
}
```

2. Create concrete test classes for each database context:
```csharp
[Collection(easyshifthqTestConsts.CollectionDefinitionName)]
public class EfCoreMyFeature_Tests : MyFeatureTests<easyshifthqEntityFrameworkCoreTestModule>
{
}
```

This pattern allows:
- Reuse of test logic across different database implementations
- Consistent test setup through protected constructors
- Proper dependency injection in test environments
- Clean separation of test logic from test infrastructure

### Best Practices
- Put common test logic in the abstract base class
- Use protected constructors for service initialization
- Follow the naming convention: `[Feature]Tests` for base classes and `EfCore[Feature]_Tests` for EF Core implementations
- Use the `[Collection]` attribute to ensure proper test isolation
- Initialize services using `GetRequiredService<T>()`

For examples, see:
- `InvitationSecurityTests` - Security-focused test implementation
- `InvitationAppServiceTests` - Application service test implementation

## 13. Code Analysis
- Follow Microsoft Code Analysis rules
- Use provided `.editorconfig` settings
- Maintain consistency across:
  - Design rules
  - Documentation rules
  - Performance rules
  - Localization rules

## 14. API Development
- Implement RESTful endpoints in HttpApi project
- Use DTOs for data transfer
- Follow API versioning conventions
- Document endpoints with OpenAPI/Swagger

## 15. Security
- Implement multi-tenancy where required
- Use proper authentication/authorization
- Follow secure coding practices
- Validate all inputs

## 16. Performance
- Follow performance best practices
- Optimize database queries
- Implement caching where appropriate
- Monitor application metrics

## 17. Documentation
- Maintain up-to-date API documentation
- Document complex business logic
- Include setup instructions
- Document configuration options

## 18. Development Workflow
- Follow Git branching strategy
- Use pull requests for code reviews
- Run tests before merging
- Keep dependencies updated

## 19. Configuration Management
- Use proper environment-specific settings
- Secure sensitive configuration
- Document configuration options
- Follow configuration best practices

## 20. Step-by-Step Application Development

### Creating a CRUD Application
Follow these steps to build a production-quality CRUD application:

1. **Define Domain Objects**
   - Create entities in the Domain layer
   - Use appropriate base classes (AuditedAggregateRoot, FullAuditedAggregateRoot)
   - Define constants for validation

```csharp
// Example: Category entity
public class Category : AuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
}

// Example: Product entity
public class Product : FullAuditedAggregateRoot<Guid>
{
    public Category Category { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public float Price { get; set; }
    public bool IsFreeCargo { get; set; }
    public DateTime ReleaseDate { get; set; }
    public ProductStockState StockState { get; set; }
}
```

2. **Configure Database**
   - Add DbSet properties to DbContext
   - Configure entity mappings using Fluent API
   - Create and apply migrations

```csharp
// Example: DbContext configuration
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);
    
    builder.Entity<Category>(b =>
    {
        b.ToTable("Categories");
        b.Property(x => x.Name)
            .HasMaxLength(CategoryConsts.MaxNameLength)
            .IsRequired();
        b.HasIndex(x => x.Name);
    });
}
```

3. **Define Application Services**
   - Create DTOs for data transfer
   - Define application service interfaces
   - Implement application services

```csharp
// Example: Application service interface
public interface IProductAppService : IApplicationService
{
    Task<PagedResultDto<ProductDto>> GetListAsync(PagedAndSortedResultRequestDto input);
    Task CreateAsync(CreateUpdateProductDto input);
    Task<ProductDto> GetAsync(Guid id);
    Task UpdateAsync(Guid id, CreateUpdateProductDto input);
    Task DeleteAsync(Guid id);
}
```

4. **Create UI Components**
   - Use Razor Pages for views
   - Implement page models
   - Use ABP tag helpers

```csharp
// Example: Razor Page model
public class IndexModel : EasyShiftHQPageModel
{
    private readonly IProductAppService _productAppService;

    public IndexModel(IProductAppService productAppService)
    {
        _productAppService = productAppService;
    }

    public async Task OnGetAsync()
    {
        // Page initialization logic
    }
}
```

5. **Implement Data Tables**
   - Use ABP's DataTables integration
   - Configure columns and actions
   - Handle CRUD operations

```javascript
// Example: DataTable configuration
var dataTable = $('#ProductsTable').DataTable(
    abp.libs.datatables.normalizeConfiguration({
        serverSide: true,
        paging: true,
        order: [[0, "asc"]],
        searching: false,
        scrollX: true,
        ajax: abp.libs.datatables.createAjax(
            productManagement.products.product.getList),
        columnDefs: [
            {
                title: l('Name'),
                data: "name"
            },
            // ... other columns
        ]
    })
);
```

6. **Add Modal Dialogs**
   - Create modal pages for create/edit
   - Use dynamic forms
   - Handle form submission

```csharp
// Example: Modal dialog
<abp-dynamic-form abp-model="Product"
                  asp-page="/Products/CreateProductModal">
    <abp-modal>
        <abp-modal-header title="@L["NewProduct"].Value"></abp-modal-header>
        <abp-modal-body>
            <abp-form-content />
        </abp-modal-body>
        <abp-modal-footer buttons="@(AbpModalButtons.Cancel|AbpModalButtons.Save)"></abp-modal-footer>
    </abp-modal>
</abp-dynamic-form>
```

### Best Practices
1. **Domain Layer**
   - Keep entities clean and focused
   - Use appropriate base classes
   - Define constants for validation

2. **Application Layer**
   - Use DTOs for data transfer
   - Implement async methods
   - Handle validation and authorization

3. **UI Layer**
   - Use ABP tag helpers
   - Implement proper error handling
   - Follow responsive design principles

4. **Testing**
   - Write unit tests for application services
   - Test domain logic
   - Verify UI functionality

5. **Security**
   - Implement proper authorization
   - Validate all inputs
   - Use secure communication

### Common Patterns
1. **Soft Delete**
   - Inherit from FullAuditedAggregateRoot
   - Use IsDeleted property
   - Configure automatic filtering

2. **Audit Logging**
   - Use AuditedAggregateRoot
   - Track creation and modification
   - Log important changes

3. **Object Mapping**
   - Use AutoMapper
   - Define mapping profiles
   - Handle complex mappings

4. **Localization**
   - Use ABP's localization system
   - Define resource files
   - Support multiple languages

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 21. ASP.NET Core and ABP Infrastructure

### Module System
ABP Framework provides a modular architecture that extends ASP.NET Core's startup system:

1. **Module Definition**
```csharp
[DependsOn(typeof(ModuleB), typeof(ModuleC))]
public class ModuleA : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        // Register services
        context.Services.AddTransient<MyService>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        
        app.UseRouting();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseEndpoints(endpoints => {
            endpoints.MapControllers();
        });
    }
}
```

2. **Module Lifecycle Methods**
   - PreConfigureServices: Called before ConfigureServices
   - ConfigureServices: Main method for service registration
   - PostConfigureServices: Called after all ConfigureServices
   - OnPreApplicationInitialization: Before OnApplicationInitialization
   - OnApplicationInitialization: Configure request pipeline
   - OnPostApplicationInitialization: After initialization
   - OnApplicationShutdown: Module shutdown logic

### Dependency Injection
ABP extends ASP.NET Core's DI system with automatic registration:

1. **Service Registration**
```csharp
// Manual registration
context.Services.AddTransient<ISmsService, SmsService>();
context.Services.AddSingleton<OtherService>();

// Automatic registration (no need to register)
public class AzureSmsService : ISmsService, ITransientDependency
{
    // Automatically registered as transient
}

// Using Dependency attribute
[Dependency(ServiceLifetime.Transient, TryRegister = true)]
public class UserPermissionCache
{
    // Fine-grained control over registration
}
```

2. **Service Lifetimes**
   - Transient: New instance per injection
   - Scoped: One instance per request
   - Singleton: Single instance for application

3. **Exposing Services**
```csharp
[ExposeServices(typeof(IPdfExporter))]
public class PdfExporter: IExporter, IPdfExporter, ICanExport, ITransientDependency
{
    // Only exposed through IPdfExporter interface
}
```

### Configuration System
ABP uses ASP.NET Core's configuration system with additional features:

1. **Reading Configuration**
```csharp
public class AzureSmsService : ISmsService, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public AzureSmsService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        string sender = _configuration["AzureSmsService:Sender"];
        string connectionString = _configuration["AzureSmsService:ConnectionString"];
    }
}
```

2. **Options Pattern**
```csharp
// Define options class
public class AzureSmsServiceOptions
{
    public string Sender { get; set; }
    public string ConnectionString { get; set; }
}

// Configure options
public override void ConfigureServices(ServiceConfigurationContext context)
{
    Configure<AzureSmsServiceOptions>(options =>
    {
        options.Sender = "+901112223344";
        options.ConnectionString = "...";
    });
}

// Use options
public class AzureSmsService : ISmsService, ITransientDependency
{
    private readonly AzureSmsServiceOptions _options;

    public AzureSmsService(IOptions<AzureSmsServiceOptions> options)
    {
        _options = options.Value;
    }
}
```

### Logging
ABP uses ASP.NET Core's logging system with Serilog by default:

```csharp
public class AzureSmsService : ISmsService, ITransientDependency
{
    private readonly ILogger<AzureSmsService> _logger;

    public AzureSmsService(ILogger<AzureSmsService> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(string phoneNumber, string message)
    {
        _logger.LogInformation($"Sending SMS to {phoneNumber}: {message}");
        // Implementation
    }
}
```

### Best Practices
1. **Module Design**
   - Keep modules focused and cohesive
   - Use proper dependency declarations
   - Follow module lifecycle methods

2. **Service Registration**
   - Use automatic registration when possible
   - Choose appropriate service lifetime
   - Implement interfaces for loose coupling

3. **Configuration**
   - Use options pattern for type-safe configuration
   - Set options from configuration by default
   - Document configuration options

4. **Logging**
   - Use appropriate log levels
   - Include relevant context in log messages
   - Consider performance impact

## 22. Working with Data Access Infrastructure

### Entity Definitions
ABP Framework provides standardized base classes for defining entities:

1. **Aggregate Root Classes**
```csharp
// Basic aggregate root
public class Form : BasicAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsDraft { get; set; }
    public ICollection<Question> Questions { get; set; }
}

// Audited aggregate root
public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    // Includes audit properties: CreationTime, CreatorId, LastModificationTime, etc.
}
```

2. **Entity Classes**
```csharp
// Basic entity
public class Question : Entity<Guid>
{
    public Guid FormId { get; set; }
    public string Title { get; set; }
    public bool AllowMultiSelect { get; set; }
    public ICollection<Option> Options { get; set; }
}

// Audited entity
public class OrderItem : FullAuditedEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
```

3. **Composite Primary Keys**
```csharp
public class FormManager : Entity
{
    public Guid FormId { get; set; }
    public Guid UserId { get; set; }
    public bool IsOwner { get; set; }

    public override object[] GetKeys()
    {
        return new object[] { FormId, UserId };
    }
}
```

### Working with Repositories
ABP provides generic repositories with built-in methods:

1. **Basic Repository Usage**
```csharp
public class FormService : ITransientDependency
{
    private readonly IRepository<Form, Guid> _formRepository;

    public FormService(IRepository<Form, Guid> formRepository)
    {
        _formRepository = formRepository;
    }

    public async Task<List<Form>> GetDraftForms()
    {
        return await _formRepository.GetListAsync(f => f.IsDraft);
    }
}
```

2. **CRUD Operations**
```csharp
// Insert
await _formRepository.InsertAsync(new Form { Name = "Test Form" });

// Update
var form = await _formRepository.GetAsync(id);
form.Name = "Updated Name";
await _formRepository.UpdateAsync(form);

// Delete
await _formRepository.DeleteAsync(id);
```

3. **Advanced Querying**
```csharp
// Using LINQ
var queryable = await _formRepository.GetQueryableAsync();
var forms = await queryable
    .Where(f => f.Name.Contains("test"))
    .OrderBy(f => f.Name)
    .ToListAsync();

// Using repository extensions
var count = await _formRepository.CountAsync(f => f.IsDraft);
var exists = await _formRepository.AnyAsync(f => f.Name == "Test");
```

### Entity Framework Core Integration

1. **DbContext Configuration**
```csharp
public class FormsAppDbContext : AbpDbContext<FormsAppDbContext>
{
    public DbSet<Form> Forms { get; set; }
    public DbSet<Question> Questions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Form>(b =>
        {
            b.ToTable("Forms");
            b.ConfigureByConvention();
            b.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired();
            b.HasIndex(x => x.Name);
        });
    }
}
```

2. **Custom Repository Implementation**
```csharp
public class FormRepository : EfCoreRepository<FormsAppDbContext, Form, Guid>, IFormRepository
{
    public FormRepository(IDbContextProvider<FormsAppDbContext> dbContextProvider)
        : base(dbContextProvider)
    { }

    public async Task<List<Form>> GetListAsync(string name, bool includeDrafts = false)
    {
        var dbContext = await GetDbContextAsync();
        var query = dbContext.Forms
            .Where(f => f.Name.Contains(name))
            .WhereIf(!includeDrafts, f => !f.IsDraft);
        return await query.ToListAsync();
    }
}
```

### Unit of Work System

1. **Default Behavior**
- HTTP requests are automatically wrapped in a UoW
- GET requests don't use transactions
- Other HTTP methods use transactions
- Changes are committed on success, rolled back on failure

2. **Configuration**
```csharp
Configure<AbpUnitOfWorkDefaultOptions>(options =>
{
    options.TransactionBehavior = UnitOfWorkTransactionBehavior.Auto;
    options.Timeout = 300000; // 5 minutes
    options.IsolationLevel = IsolationLevel.Serializable;
});
```

3. **Manual Control**
```csharp
[UnitOfWork(isTransactional: true)]
public async Task DoItAsync()
{
    await _formRepository.InsertAsync(new Form());
    await _formRepository.InsertAsync(new Form());
}

// Or using IUnitOfWorkManager
public async Task DoItAsync()
{
    using (var uow = _unitOfWorkManager.Begin(
        requiresNew: true,
        isTransactional: true,
        timeout: 15000))
    {
        await _formRepository.InsertAsync(new Form());
        await uow.CompleteAsync();
    }
}
```

### Best Practices
1. **Entity Design**
   - Use appropriate base classes
   - Implement proper validation
   - Define clear relationships
   - Use GUIDs for primary keys

2. **Repository Usage**
   - Prefer generic repositories
   - Create custom repositories for complex queries
   - Use async/await pattern
   - Handle exceptions properly

3. **Data Access**
   - Use transactions appropriately
   - Optimize queries
   - Implement proper error handling
   - Consider performance implications

4. **Unit of Work**
   - Let ABP handle UoW for HTTP requests
   - Manually control UoW for background jobs
   - Set appropriate timeouts
   - Handle transaction isolation levels

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 23. Cross-Cutting Concerns

### Authorization and Permissions

1. **Simple Authorization**
```csharp
[Authorize]
public class ProductController : Controller
{
    [AllowAnonymous]
    public async Task<List<ProductDto>> GetListAsync()
    {
        // Available to everyone
    }

    public async Task CreateAsync(ProductCreationDto input)
    {
        // Requires authentication
    }
}
```

2. **Permission System**
```csharp
// Define permissions
public class ProductManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(
            "ProductManagement",
            L("ProductManagement"));

        myGroup.AddPermission(
            "ProductManagement.ProductCreation",
            L("ProductCreation"));

        myGroup.AddPermission(
            "ProductManagement.ProductDeletion",
            L("ProductDeletion"));
    }
}

// Use permissions
[Authorize("ProductManagement.ProductCreation")]
public async Task CreateAsync(ProductCreationDto input)
{
    // Implementation
}
```

3. **Programmatic Permission Checks**
```csharp
public class ProductService : ITransientDependency
{
    private readonly IAuthorizationService _authorizationService;

    public async Task CreateAsync(ProductCreationDto input)
    {
        await _authorizationService.CheckAsync(
            "ProductManagement.ProductCreation");
        // Implementation
    }
}
```

### Input Validation

1. **Data Annotation Attributes**
```csharp
public class ProductCreationDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Range(0, 999.99)]
    public decimal Price { get; set; }

    [Url]
    public string PictureUrl { get; set; }
}
```

2. **Custom Validation**
```csharp
public class ProductCreationDto : IValidatableObject
{
    public bool IsDraft { get; set; }
    public string PictureUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(
        ValidationContext context)
    {
        if (IsDraft == false && string.IsNullOrEmpty(PictureUrl))
        {
            yield return new ValidationResult(
                "Picture must be provided to publish a product",
                new[] { nameof(PictureUrl) }
            );
        }
    }
}
```

3. **FluentValidation Integration**
```csharp
public class ProductCreationDtoValidator : AbstractValidator<ProductCreationDto>
{
    public ProductCreationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .ExclusiveBetween(0, 1000);

        RuleFor(x => x.PictureUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.PictureUrl));
    }
}
```

### Exception Handling

1. **User-Friendly Exceptions**
```csharp
public async Task CreateProductAsync(ProductCreationDto input)
{
    if (await HasExistingProductAsync(input.Name))
    {
        throw new UserFriendlyException(
            "A product with this name already exists!");
    }
    // Implementation
}
```

2. **Business Exceptions**
```csharp
public class ProductNameAlreadyExistsException : BusinessException
{
    public string Name { get; private set; }

    public ProductNameAlreadyExistsException(string name)
        : base("ProductManagement:ProductNameAlreadyExists")
    {
        Name = name;
        WithData("Name", name);
    }
}

// Localization
{
    "culture": "en",
    "texts": {
        "ProductManagement:ProductNameAlreadyExists": 
            "The product name '{Name}' already exists. Please use another name."
    }
}
```

3. **Exception Logging**
```csharp
public class ProductException : Exception, IHasLogLevel
{
    public LogLevel LogLevel { get; set; } = LogLevel.Warning;
    
    public ProductException(string message) : base(message)
    {
    }
}

public class DetailedProductException : Exception, IExceptionWithSelfLogging
{
    public void Log(ILogger logger)
    {
        logger.LogWarning("Additional details: {Details}", 
            GetExceptionDetails());
    }
}
```

4. **HTTP Status Code Mapping**
```csharp
services.Configure<AbpExceptionHttpStatusCodeOptions>(options =>
{
    options.Map(
        "ProductManagement:ProductNameAlreadyExists",
        HttpStatusCode.Conflict);
});
```

### Best Practices

1. **Authorization**
   - Use declarative authorization with attributes when possible
   - Define clear permission hierarchies
   - Implement proper role-based access control
   - Consider multi-tenancy in permission design

2. **Validation**
   - Validate at multiple levels (client, server, business)
   - Use appropriate validation attributes
   - Implement custom validation for complex rules
   - Consider using FluentValidation for complex scenarios

3. **Exception Handling**
   - Use appropriate exception types
   - Provide meaningful error messages
   - Implement proper logging
   - Consider localization needs
   - Map exceptions to appropriate HTTP status codes

4. **Cross-Cutting Concerns**
   - Keep validation logic close to the data
   - Implement proper error handling at all layers
   - Use consistent authorization patterns
   - Consider performance implications
   - Document security requirements

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 24. Additional ABP Framework Features

### Current User Information

1. **Basic Usage**
```csharp
public class MyService : ITransientDependency
{
    private readonly ICurrentUser _currentUser;

    public MyService(ICurrentUser currentUser)
    {
        _currentUser = currentUser;
    }

    public void Demo()
    {
        Guid? userId = _currentUser.Id;
        string userName = _currentUser.UserName;
        string email = _currentUser.Email;
        bool isAuthenticated = _currentUser.IsAuthenticated;
    }
}
```

2. **Custom Claims**
```csharp
public class SocialSecurityNumberClaimsPrincipalContributor
    : IAbpClaimsPrincipalContributor, ITransientDependency
{
    public async Task ContributeAsync(
        AbpClaimsPrincipalContributorContext context)
    {
        ClaimsIdentity identity = context.ClaimsPrincipal
            .Identities.FirstOrDefault();
        var userId = identity?.FindUserId();
        
        if (userId.HasValue)
        {
            var userService = context.ServiceProvider
                .GetRequiredService<IUserService>();
            var socialSecurityNumber = await userService
                .GetSocialSecurityNumberAsync(userId.Value);
                
            if (socialSecurityNumber != null)
            {
                identity.AddClaim(new Claim(
                    "SocialSecurityNumber", 
                    socialSecurityNumber));
            }
        }
    }
}
```

### Data Filtering

1. **Soft Delete Filter**
```csharp
public class Order : AggregateRoot<Guid>, ISoftDelete
{
    public bool IsDeleted { get; set; }
    // ... other properties
}
```

2. **Multi-Tenancy Filter**
```csharp
public class Order : AggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    // ... other properties
}
```

3. **Custom Data Filter**
```csharp
public interface IArchivable
{
    bool IsArchived { get; }
}

public class Order : AggregateRoot<Guid>, IArchivable
{
    public bool IsArchived { get; set; }
}

// DbContext Configuration
protected override bool ShouldFilterEntity<TEntity>(
    IMutableEntityType entityType)
{
    if (typeof(IArchivable)
        .IsAssignableFrom(typeof(TEntity)))
    {
        return true;
    }
    return base.ShouldFilterEntity<TEntity>(entityType);
}
```

### Audit Logging

1. **Entity History**
```csharp
[Audited]
public class Order : AggregateRoot<Guid>
{
    public string OrderNumber { get; set; }
    
    [DisableAuditing]
    public string CreditCardNumber { get; set; }
}
```

2. **Service Auditing**
```csharp
[DisableAuditing]
public class OrderAppService : ApplicationService
{
    [Audited]
    public async Task CreateAsync(CreateOrderDto input)
    {
        // Implementation
    }
}
```

3. **Audit Log Configuration**
```csharp
Configure<AbpAuditingOptions>(options =>
{
    options.IsEnabled = true;
    options.IsEnabledForGetRequests = false;
    options.IsEnabledForAnonymousUsers = true;
    options.EntityHistorySelectors.AddAllEntities();
});
```

### Caching

1. **Distributed Cache**
```csharp
public class UserCacheItem
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string EmailAddress { get; set; }
}

public class MyUserService : ITransientDependency
{
    private readonly IDistributedCache<UserCacheItem> _userCache;

    public async Task<UserCacheItem> GetUserInfoAsync(Guid userId)
    {
        return await _userCache.GetOrAddAsync(
            userId.ToString(),
            async () => await GetUserFromDatabaseAsync(userId),
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            }
        );
    }
}
```

2. **Cache Configuration**
```csharp
Configure<AbpDistributedCacheOptions>(options =>
{
    options.GlobalCacheEntryOptions
        .AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);
});
```

3. **Cache Invalidation**
```csharp
public class MyUserService : 
    ILocalEventHandler<EntityChangedEventData<IdentityUser>>,
    ITransientDependency
{
    private readonly IDistributedCache<UserCacheItem> _userCache;

    public async Task HandleEventAsync(
        EntityChangedEventData<IdentityUser> data)
    {
        await _userCache.RemoveAsync(data.Entity.Id.ToString());
    }
}
```

### Localization

1. **Language Configuration**
```csharp
Configure<AbpLocalizationOptions>(options =>
{
    options.Languages.Add(new LanguageInfo("en", "en", "English"));
    options.Languages.Add(new LanguageInfo("es", "es", "Español"));
});
```

2. **Localization Resource**
```csharp
[LocalizationResourceName("DemoApp")]
public class DemoAppResource
{ }

// Localization JSON
{
    "culture": "en",
    "texts": {
        "WelcomeMessage": "Welcome to the application",
        "WelcomeMessageWithName": "Welcome {0} to the application"
    }
}
```

3. **Using Localization**
```csharp
public class LocalizationDemoService : ITransientDependency
{
    private readonly IStringLocalizer<DemoAppResource> _localizer;

    public string GetWelcomeMessage(string name)
    {
        return _localizer["WelcomeMessageWithName", name];
    }
}
```

### Best Practices

1. **Current User**
   - Use ICurrentUser for user information
   - Implement custom claims for additional user data
   - Consider performance implications
   - Handle null cases properly

2. **Data Filtering**
   - Use appropriate filter interfaces
   - Implement custom filters when needed
   - Consider performance impact
   - Document filter behavior

3. **Audit Logging**
   - Enable entity history selectively
   - Exclude sensitive data
   - Configure appropriate options
   - Monitor log size

4. **Caching**
   - Use type-safe cache items
   - Set appropriate expiration times
   - Implement proper cache invalidation
   - Consider distributed scenarios

5. **Localization**
   - Define clear resource structure
   - Use parameterized texts
   - Consider fallback behavior
   - Document localization keys

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 25. Domain-Driven Design with ABP Framework

### DDD Layers and Building Blocks

1. **Domain Layer**
   - Contains core business objects and logic
   - Independent of other layers
   - Key components:
     - Entities
     - Value Objects
     - Aggregates
     - Repositories
     - Domain Services
     - Domain Events
     - Specifications

2. **Application Layer**
   - Implements use cases
   - Uses domain layer objects
   - Key components:
     - Application Services
     - DTOs
     - Unit of Work

3. **Presentation Layer**
   - Contains UI components
   - Uses application layer
   - No direct domain layer access

4. **Infrastructure Layer**
   - Implements abstractions
   - Depends on other layers
   - Handles technical concerns

### Solution Structure

1. **Basic DDD Solution**
```plaintext
Acme.Crm.Domain/           # Domain layer
Acme.Crm.Infrastructure/   # Infrastructure layer
Acme.Crm.Application/      # Application layer
Acme.Crm.Web/             # Presentation layer
```

2. **ABP Framework Solution**
```plaintext
Acme.Crm.Domain/           # Domain layer
Acme.Crm.Domain.Shared/    # Shared domain types
Acme.Crm.EntityFrameworkCore/ # EF Core integration
Acme.Crm.Application/      # Application layer
Acme.Crm.Application.Contracts/ # Service contracts
Acme.Crm.HttpApi/          # HTTP API layer
Acme.Crm.HttpApi.Client/   # API client
Acme.Crm.Web/             # Presentation layer
Acme.Crm.DbMigrator/      # Database migrations
```

### Domain Layer Implementation

1. **Entity Definition**
```csharp
public class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public float Price { get; private set; }
    public ProductStockState StockState { get; private set; }

    private Product() { } // For EF Core

    public Product(Guid id, string name, float price)
    {
        Id = id;
        SetName(name);
        SetPrice(price);
        StockState = ProductStockState.PreOrder;
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name, 
            nameof(name),
            maxLength: ProductConsts.MaxNameLength
        );
    }

    public void SetPrice(float price)
    {
        Price = Check.Positive(
            price, 
            nameof(price)
        );
    }
}
```

2. **Value Object**
```csharp
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string Country { get; private set; }

    private Address() { }

    public Address(string street, string city, string country)
    {
        Street = street;
        City = city;
        Country = country;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return Country;
    }
}
```

3. **Repository Interface**
```csharp
public interface IProductRepository : IRepository<Product, Guid>
{
    Task<List<Product>> GetListAsync(
        string name,
        bool includeDrafts = false
    );

    Task<Product> GetByNameAsync(string name);
}
```

4. **Domain Service**
```csharp
public class ProductManager : DomainService
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> CreateAsync(
        string name,
        float price)
    {
        await CheckDuplicateNameAsync(name);

        return new Product(
            GuidGenerator.Create(),
            name,
            price
        );
    }

    private async Task CheckDuplicateNameAsync(string name)
    {
        var existingProduct = await _productRepository
            .GetByNameAsync(name);
            
        if (existingProduct != null)
        {
            throw new BusinessException(
                "ProductManagement:ProductNameAlreadyExists"
            );
        }
    }
}
```

### Application Layer Implementation

1. **Application Service**
```csharp
public class ProductAppService : ApplicationService
{
    private readonly IProductRepository _productRepository;
    private readonly ProductManager _productManager;

    public ProductAppService(
        IProductRepository productRepository,
        ProductManager productManager)
    {
        _productRepository = productRepository;
        _productManager = productManager;
    }

    public async Task<ProductDto> CreateAsync(
        CreateProductDto input)
    {
        var product = await _productManager.CreateAsync(
            input.Name,
            input.Price
        );

        await _productRepository.InsertAsync(product);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }
}
```

2. **DTO Definition**
```csharp
public class ProductDto : AuditedEntityDto<Guid>
{
    public string Name { get; set; }
    public float Price { get; set; }
    public ProductStockState StockState { get; set; }
}

public class CreateProductDto
{
    [Required]
    [StringLength(ProductConsts.MaxNameLength)]
    public string Name { get; set; }

    [Range(0, float.MaxValue)]
    public float Price { get; set; }
}
```

### Best Practices

1. **Domain Layer**
   - Keep entities focused and cohesive
   - Implement proper validation
   - Use value objects for complex types
   - Define clear aggregate boundaries
   - Implement domain services for cross-aggregate logic
   - Use repositories for data access

2. **Application Layer**
   - Keep application services thin
   - Use DTOs for data transfer
   - Implement proper validation
   - Handle transactions properly
   - Use domain services for business logic
   - Consider performance implications

3. **Presentation Layer**
   - Keep UI logic separate
   - Use application services
   - Implement proper error handling
   - Consider user experience
   - Follow responsive design principles

4. **Infrastructure Layer**
   - Implement proper abstractions
   - Handle technical concerns
   - Consider performance
   - Implement proper error handling
   - Follow security best practices

5. **General Principles**
   - Keep layers independent
   - Follow SOLID principles
   - Implement proper validation
   - Handle errors gracefully
   - Consider performance
   - Document complex logic
   - Write unit tests

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 26. Domain-Driven Design Principles

### DDD Layers and Building Blocks

1. **Domain Layer**
   - Contains core business objects and logic
   - Independent of other layers
   - Key components:
     - Entities
     - Value Objects
     - Aggregates
     - Repositories
     - Domain Services
     - Domain Events
     - Specifications

2. **Application Layer**
   - Implements use cases
   - Uses domain layer objects
   - Key components:
     - Application Services
     - DTOs
     - Unit of Work

3. **Presentation Layer**
   - Contains UI components
   - Uses application layer
   - No direct domain layer access

4. **Infrastructure Layer**
   - Implements abstractions
   - Depends on other layers
   - Handles technical concerns

### Aggregate Design Principles

1. **Aggregate Root**
   - Single entry point for aggregate
   - Maintains consistency boundaries
   - Controls access to child entities
   - Example:
```csharp
public class Event : FullAuditedAggregateRoot<Guid>
{
    public string Title { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public ICollection<Track> Tracks { get; private set; }

    private Event() { } // For EF Core

    public Event(Guid id, string title, DateTime startTime, DateTime endTime)
    {
        Id = id;
        SetTitle(title);
        SetTime(startTime, endTime);
        Tracks = new Collection<Track>();
    }

    public void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(
            title, 
            nameof(title),
            maxLength: EventConsts.MaxTitleLength
        );
    }

    public void SetTime(DateTime startTime, DateTime endTime)
    {
        if (startTime > endTime)
        {
            throw new BusinessException(
                EventHubErrorCodes.EndTimeCantBeEarlierThanStartTime
            );
        }
        StartTime = startTime;
        EndTime = endTime;
    }
}
```

2. **Entity Design**
   - Private setters for properties
   - Public methods for state changes
   - Validation in methods
   - Example:
```csharp
public class Track : Entity<Guid>
{
    public string Name { get; private set; }
    public ICollection<Session> Sessions { get; private set; }

    private Track() { }

    public Track(Guid id, string name)
    {
        Id = id;
        SetName(name);
        Sessions = new Collection<Session>();
    }

    public void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(
            name,
            nameof(name),
            maxLength: TrackConsts.MaxNameLength
        );
    }
}
```

3. **Value Objects**
   - Immutable
   - No identity
   - Example:
```csharp
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string Country { get; }

    public Address(string street, string city, string country)
    {
        Street = street;
        City = city;
        Country = country;
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Street;
        yield return City;
        yield return Country;
    }
}
```

### Domain Services

1. **Purpose**
   - Handle cross-aggregate operations
   - Use external services
   - Implement complex business rules
   - Example:
```csharp
public class EventManager : DomainService
{
    private readonly IRepository<Event, Guid> _eventRepository;
    private readonly IRepository<EventRegistration, Guid> _registrationRepository;

    public EventManager(
        IRepository<Event, Guid> eventRepository,
        IRepository<EventRegistration, Guid> registrationRepository)
    {
        _eventRepository = eventRepository;
        _registrationRepository = registrationRepository;
    }

    public async Task SetCapacityAsync(Event @event, int? capacity)
    {
        if (capacity.HasValue)
        {
            var registeredUserCount = await _registrationRepository
                .CountAsync(x => x.EventId == @event.Id);
                
            if (capacity.Value < registeredUserCount)
            {
                throw new BusinessException(
                    EventHubErrorCodes.CapacityCanNotBeLowerThanRegisteredUserCount
                );
            }
        }
        @event.Capacity = capacity;
    }
}
```

2. **Best Practices**
   - Keep services focused
   - Use dependency injection
   - Handle exceptions properly
   - Document business rules

### Repositories

1. **Purpose**
   - Abstract data access
   - Handle aggregate persistence
   - Example:
```csharp
public interface IEventRepository : IRepository<Event, Guid>
{
    Task<List<Event>> GetUpcomingEventsAsync();
    Task<List<Event>> GetEventsByOrganizationAsync(Guid organizationId);
}
```

2. **Implementation**
```csharp
public class EventRepository : EfCoreRepository<EventHubDbContext, Event, Guid>, IEventRepository
{
    public EventRepository(IDbContextProvider<EventHubDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Event>> GetUpcomingEventsAsync()
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.Events
            .Where(x => x.StartTime > Clock.Now)
            .OrderBy(x => x.StartTime)
            .ToListAsync();
    }
}
```

### Specifications

1. **Purpose**
   - Encapsulate query logic
   - Reusable filters
   - Example:
```csharp
public class UpcomingEventSpecification : Specification<Event>
{
    public override Expression<Func<Event, bool>> ToExpression()
    {
        return x => x.StartTime > Clock.Now;
    }
}
```

2. **Combining Specifications**
```csharp
var specification = new UpcomingEventSpecification()
    .And(new OnlineEventSpecification())
    .And(new SpeakerSpecification(userId));
```

### Domain Events

1. **Local Events**
   - Same process
   - Same transaction
   - Example:
```csharp
public class EventTimeChangedEventData
{
    public Event Event { get; }

    public EventTimeChangedEventData(Event @event)
    {
        Event = @event;
    }
}

public class UserEmailingHandler : 
    ILocalEventHandler<EventTimeChangedEventData>,
    ITransientDependency
{
    public async Task HandleEventAsync(EventTimeChangedEventData eventData)
    {
        var @event = eventData.Event;
        // Send email to registered users
    }
}
```

2. **Distributed Events**
   - Cross-process
   - Asynchronous
   - Example:
```csharp
public class EventTimeChangedEto
{
    public Guid EventId { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class UserEmailingHandler : 
    IDistributedEventHandler<EventTimeChangedEto>,
    ITransientDependency
{
    public async Task HandleEventAsync(EventTimeChangedEto eventData)
    {
        // Send email to registered users
    }
}
```

### Best Practices

1. **Aggregate Design**
   - Keep aggregates small
   - Reference other aggregates by ID
   - Maintain consistency boundaries
   - Use private setters
   - Implement validation in methods

2. **Domain Services**
   - Keep services focused
   - Use dependency injection
   - Handle exceptions properly
   - Document business rules

3. **Repositories**
   - Abstract data access
   - Handle aggregate persistence
   - Use specifications for queries
   - Keep business logic out

4. **Domain Events**
   - Use for side effects
   - Keep event data minimal
   - Handle exceptions properly
   - Consider performance

5. **General Principles**
   - Keep layers independent
   - Follow SOLID principles
   - Implement proper validation
   - Handle errors gracefully
   - Consider performance
   - Document complex logic
   - Write unit tests

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 27. HTTP APIs and Real-Time Services

### Building HTTP APIs

1. **Auto API Controllers**
ABP Framework automatically converts application services to HTTP API endpoints. For example:

```csharp
// Application service interface
public interface IInvitationAppService : IApplicationService
{
    Task<InvitationDto> CreateAsync(CreateInvitationDto input);
    Task<List<InvitationDto>> GetPendingAsync();
    Task<InvitationDto> AcceptAsync(Guid id);
}

// Automatically exposed as HTTP endpoints:
// POST /api/app/invitation
// GET /api/app/invitation/pending
// PUT /api/app/invitation/{id}/accept
```

2. **Manual API Controllers**
For custom HTTP endpoints, create controllers:

```csharp
[ApiController]
[Route("api/v1/invitations")]
public class InvitationController : AbpControllerBase
{
    private readonly IInvitationAppService _invitationAppService;

    public InvitationController(IInvitationAppService invitationAppService)
    {
        _invitationAppService = invitationAppService;
    }

    [HttpGet("pending")]
    public async Task<List<InvitationDto>> GetPendingAsync()
    {
        return await _invitationAppService.GetPendingAsync();
    }
}
```

3. **API Versioning**
Configure API versioning in your module:

```csharp
Configure<AbpAspNetCoreMvcOptions>(options =>
{
    options.ConventionalControllers.Create(
        typeof(YourApplicationModule).Assembly,
        opts =>
        {
            opts.RootPath = "api/v1";
            opts.RemoteServiceName = "YourService";
        }
    );
});
```

### Real-Time Services with SignalR

1. **SignalR Hub Implementation**
```csharp
public class NotificationHub : AbpHub
{
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task SendInvitationNotification(string email)
    {
        await Clients.All.SendAsync("ReceiveInvitationNotification", email);
    }
}
```

2. **Client-Side SignalR Integration**
```javascript
$(function () {
    var connection = new signalR.HubConnectionBuilder()
        .withUrl('/signalr-hubs/notification')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceiveNotification', function (message) {
        abp.notify.info(message);
    });

    connection.on('ReceiveInvitationNotification', function (email) {
        abp.notify.success('New invitation sent to: ' + email);
    });

    connection.start()
        .then(function () {
            console.log('SignalR Connected!');
        })
        .catch(function (err) {
            console.error(err.toString());
        });
});
```

3. **Server-Side SignalR Usage**
```csharp
public class InvitationAppService : ApplicationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public InvitationAppService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task<InvitationDto> CreateAsync(CreateInvitationDto input)
    {
        var invitation = await _invitationRepository.InsertAsync(
            new Invitation(
                GuidGenerator.Create(),
                input.Email,
                input.FirstName,
                input.LastName
            )
        );

        await _hubContext.Clients.All.SendAsync(
            "ReceiveInvitationNotification",
            input.Email
        );

        return ObjectMapper.Map<Invitation, InvitationDto>(invitation);
    }
}
```

### Best Practices

1. **HTTP APIs**
   - Use Auto API Controllers when possible
   - Follow RESTful conventions
   - Implement proper validation
   - Use appropriate HTTP methods
   - Document endpoints with Swagger
   - Handle errors consistently
   - Implement proper authorization

2. **SignalR**
   - Use appropriate hub methods
   - Handle connection errors
   - Implement reconnection logic
   - Use proper authorization
   - Consider performance implications
   - Handle message size limits
   - Implement proper error handling

3. **Security**
   - Implement proper authentication
   - Use HTTPS for all communications
   - Validate all inputs
   - Implement rate limiting
   - Use proper authorization
   - Handle sensitive data properly

4. **Performance**
   - Use async/await pattern
   - Implement proper caching
   - Optimize database queries
   - Handle connection pooling
   - Monitor performance metrics
   - Implement proper logging

5. **Testing**
   - Test unit tests for controllers
   - Test SignalR hubs
   - Test client-side integration
   - Test error scenarios
   - Test authorization
   - Test performance

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 28. Working with Modularity

### Understanding Modularity in ABP Framework

1. **Module Types**
   - **Class Libraries (NuGet Packages)**
     - Basic building blocks of modularity
     - Can be published as NuGet packages
     - Example: `Volo.Abp.Validation`, `Volo.Abp.Authorization`
   - **Application Modules**
     - Vertical slices of an application
     - Can include multiple packages
     - Example: Account, Identity, Tenant Management modules

2. **Module Isolation Levels**
   ```csharp
   // Tightly Coupled Module
   public class TightlyCoupledModule : AbpModule
   {
       // Shares database and entities with other modules
   }

   // Bounded Context Module
   public class BoundedContextModule : AbpModule
   {
       // Hides internal domain objects
       // Uses integration services
       // Can use different DBMS
   }

   // Generic Module
   public class GenericModule : AbpModule
   {
       // Application-independent
       // Provides customization points
       // Example: Payment module
   }

   // Plugin Module
   public class PluginModule : AbpModule
   {
       // Completely isolated
       // Optional and removable
       // Implements standard abstractions
   }
   ```

### Creating a New Module

1. **Using ABP CLI**
   ```bash
   abp new Payment -t module
   ```

2. **Module Structure**
   ```plaintext
   Payment/
   ├── src/
   │   ├── Payment.Domain/
   │   ├── Payment.Domain.Shared/
   │   ├── Payment.EntityFrameworkCore/
   │   ├── Payment.Application/
   │   ├── Payment.Application.Contracts/
   │   ├── Payment.HttpApi/
   │   ├── Payment.HttpApi.Client/
   │   └── Payment.Web/
   └── test/
       ├── Payment.Domain.Tests/
       ├── Payment.Application.Tests/
       └── Payment.Web.Tests/
   ```

3. **Module Configuration**
   ```csharp
   [DependsOn(
       typeof(AbpValidationModule),
       typeof(AbpAuthorizationModule)
   )]
   public class PaymentModule : AbpModule
   {
       public override void ConfigureServices(ServiceConfigurationContext context)
       {
           Configure<PaymentOptions>(options =>
           {
               options.DefaultCurrency = "USD";
           });
       }
   }
   ```

### Installing a Module

1. **Project Dependencies**
   ```xml
   <!-- In .csproj file -->
   <ProjectReference Include="..\..\modules\payment\src\Payment.Domain\Payment.Domain.csproj" />
   ```

2. **Module Dependencies**
   ```csharp
   [DependsOn(
       typeof(PaymentDomainModule),
       typeof(PaymentApplicationModule)
   )]
   public class YourModule : AbpModule
   {
       // Module configuration
   }
   ```

3. **Database Integration**
   ```csharp
   // Single Database Approach
   protected override void OnModelCreating(ModelBuilder builder)
   {
       base.OnModelCreating(builder);
       builder.ConfigurePayment();
   }

   // Separate Database Approach
   [ReplaceDbContext(typeof(IPaymentDbContext))]
   [ConnectionStringName(PaymentDbProperties.ConnectionStringName)]
   public class YourPaymentDbContext : AbpDbContext<YourPaymentDbContext>, IPaymentDbContext
   {
       public DbSet<PaymentRequest> PaymentRequests { get; set; }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
           modelBuilder.ConfigurePayment();
       }
   }
   ```

### Best Practices

1. **Module Design**
   - Keep modules focused and cohesive
   - Use proper dependency declarations
   - Follow module lifecycle methods
   - Implement proper validation
   - Handle errors gracefully

2. **Database Integration**
   - Choose appropriate database strategy
   - Use migrations for schema changes
   - Handle multiple database providers
   - Consider performance implications
   - Implement proper error handling

3. **Configuration**
   - Use options pattern for type-safe configuration
   - Set options from configuration by default
   - Document configuration options
   - Consider multi-tenancy
   - Handle sensitive data properly

4. **Testing**
   - Write unit tests for module components
   - Test database integration
   - Test module dependencies
   - Test error scenarios
   - Test performance

5. **Documentation**
   - Document module purpose and features
   - Document configuration options
   - Document integration points
   - Document best practices
   - Document troubleshooting steps

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 29. Implementing Multi-Tenancy

### Understanding Multi-Tenancy

1. **SaaS and Multi-Tenancy**
   - **SaaS Benefits**
     - Resource sharing between customers
     - Easy customer onboarding
     - Simplified maintenance and upgrades
     - Cost-effective for customers
   - **Multi-Tenancy Architecture**
     - Tenant: Customer using the system
     - Host: Company managing the system
     - Data isolation and security
     - Customization per tenant

2. **Database Architecture Options**
   ```csharp
   // Single Database Approach
   public class Product : AggregateRoot<Guid>, IMultiTenant
   {
       public Guid? TenantId { get; set; }
       public string Name { get; set; }
   }

   // Database Per Tenant Approach
   [ReplaceDbContext(typeof(ITenantDbContext))]
   [ConnectionStringName(TenantDbProperties.ConnectionStringName)]
   public class TenantDbContext : AbpDbContext<TenantDbContext>, ITenantDbContext
   {
       public DbSet<Product> Products { get; set; }
   }
   ```

### Working with Multi-Tenancy Infrastructure

1. **Enabling/Disabling Multi-Tenancy**
   ```csharp
   public static class MultiTenancyConsts
   {
       public const bool IsEnabled = true;
   }

   Configure<AbpMultiTenancyOptions>(options =>
   {
       options.IsEnabled = MultiTenancyConsts.IsEnabled;
   });
   ```

2. **Tenant Resolution**
   ```csharp
   Configure<AbpTenantResolveOptions>(options =>
   {
       options.AddDomainTenantResolver("{0}.yourdomain.com");
   });
   ```

3. **Working with Current Tenant**
   ```csharp
   public class MyService : ApplicationService
   {
       public async Task DoItAsync()
       {
           Guid? tenantId = CurrentTenant.Id;
           string tenantName = CurrentTenant.Name;
       }

       public async Task SwitchTenantAsync(Guid tenantId)
       {
           using (CurrentTenant.Change(tenantId))
           {
               // Work with the specified tenant's data
           }
       }
   }
   ```

### Feature System

1. **Defining Features**
   ```csharp
   public class MyFeatureDefinitionProvider : FeatureDefinitionProvider
   {
       public override void Define(IFeatureDefinitionContext context)
       {
           var myGroup = context.AddGroup("MyApp");
           
           myGroup.AddFeature(
               "MyApp.StockManagement",
               defaultValue: "false",
               displayName: L("StockManagement"),
               isVisibleToClients: true,
               valueType: new ToggleStringValueType());
       }
   }
   ```

2. **Checking Features**
   ```csharp
   // Using RequiresFeature attribute
   [RequiresFeature("MyApp.StockManagement")]
   public class StockAppService : ApplicationService
   {
   }

   // Using IFeatureChecker service
   public async Task CheckFeatureAsync()
   {
       if (await FeatureChecker.IsEnabledAsync("MyApp.StockManagement"))
       {
           // Feature is enabled
       }
   }
   ```

3. **Managing Tenant Features**
   ```csharp
   public class FeatureManager : DomainService
   {
       private readonly IFeatureManager _featureManager;

       public async Task EnableFeatureAsync(Guid tenantId)
       {
           await _featureManager.SetForTenantAsync(
               tenantId,
               "MyApp.StockManagement",
               "true"
           );
       }
   }
   ```

### Best Practices

1. **Multi-Tenancy Design**
   - Design for on-premises deployment
   - Keep tenant data isolated
   - Use proper database strategy
   - Implement proper validation
   - Handle errors gracefully

2. **Feature Management**
   - Define clear feature groups
   - Use appropriate value types
   - Implement proper validation
   - Consider performance implications
   - Document feature usage

3. **Security**
   - Implement proper authentication
   - Use proper authorization
   - Validate all inputs
   - Handle sensitive data properly
   - Consider multi-tenancy implications

4. **Performance**
   - Optimize database queries
   - Use proper caching
   - Handle connection pooling
   - Monitor performance metrics
   - Consider tenant-specific optimizations

5. **Testing**
   - Test tenant isolation
   - Test feature management
   - Test tenant switching
   - Test error scenarios
   - Test performance

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards

## 30. Building Automated Tests

### Understanding the ABP Test Infrastructure

1. **Test Project Structure**
   ```plaintext
   YourSolution/
   ├── test/
   │   ├── YourSolution.TestBase/
   │   ├── YourSolution.EntityFrameworkCore.Tests/
   │   ├── YourSolution.Domain.Tests/
   │   ├── YourSolution.Application.Tests/
   │   └── YourSolution.Web.Tests/
   ```

2. **Test Libraries**
   - xUnit: Test framework
   - Shouldly: Assertion library
   - NSubstitute: Mocking library
   - Volo.Abp.TestBase: ABP test infrastructure

### Building Unit Tests

1. **Testing Static Classes**
   ```csharp
   public class EventUrlHelper_Tests
   {
       [Fact]
       public void Should_Convert_Title_To_Proper_Urls()
       {
           var url = EventUrlHelper.ConvertTitleToUrlPart(
               "Introducing ABP Framework!");
           url.ShouldBe("introducing-abp-framework");
       }

       [Theory]
       [InlineData("Introducing ABP Framework!", "introducing-abp-framework")]
       [InlineData("Blazor: UI Messages", "blazor-ui-messages")]
       public void Should_Convert_Title_To_Proper_Urls(string title, string expectedUrl)
       {
           var result = EventUrlHelper.ConvertTitleToUrlPart(title);
           result.ShouldBe(expectedUrl);
       }
   }
   ```

2. **Testing Classes with No Dependencies**
   ```csharp
   public class Event_Tests
   {
       [Fact]
       public void Should_Create_A_Valid_Event()
       {
           new Event(
               Guid.NewGuid(),
               Guid.NewGuid(),
               "1a8j3v0d",
               "Introduction to the ABP Framework",
               DateTime.Now,
               DateTime.Now.AddHours(2),
               "In this event, we will introduce the ABP Framework..."
           );
       }

       [Fact]
       public void Should_Not_Allow_End_Time_Earlier_Than_Start_Time()
       {
           var exception = Assert.Throws<BusinessException>(() =>
           {
               new Event(
                   Guid.NewGuid(),
                   Guid.NewGuid(),
                   "1a8j3v0d",
                   "Introduction to the ABP Framework",
                   DateTime.Now,
                   DateTime.Now.AddDays(-2),
                   "In this event, we will introduce the ABP Framework..."
               );
           });
           exception.Code.ShouldBe(EventHubErrorCodes.EventEndTimeCantBeEarlierThanStartTime);
       }
   }
   ```

3. **Testing Classes with Dependencies**
   ```csharp
   public class EventRegistrationManager_UnitTests
   {
       [Fact]
       public async Task Valid_Registrations_Should_Be_Inserted_To_Db()
       {
           var evnt = new Event(/* valid arguments */);
           var user = new IdentityUser(/* valid arguments */);
           
           var repository = Substitute.For<IEventRegistrationRepository>();
           repository.ExistsAsync(evnt.Id, user.Id).Returns(Task.FromResult(false));
           
           var clock = Substitute.For<IClock>();
           clock.Now.Returns(DateTime.Now);
           
           var guidGenerator = SimpleGuidGenerator.Instance;
           var registrationManager = new EventRegistrationManager(
               repository, guidGenerator, clock
           );

           await registrationManager.RegisterAsync(evnt, user);

           await repository.Received().InsertAsync(
               Arg.Is<EventRegistration>(er => 
                   er.EventId == evnt.Id && 
                   er.UserId == user.Id)
           );
       }
   }
   ```

### Building Integration Tests

1. **ABP Integration**
   ```csharp
   public class SampleTestClass : AbpIntegratedTest<MyTestModule>
   {
       private IMyService _myService;

       public SampleTestClass()
       {
           _myService = GetRequiredService<IMyService>();
       }

       [Fact]
       public async Task TestMethod()
       {
           await _myService.DoItAsync();
       }
   }
   ```

2. **Database Integration**
   ```csharp
   public class EventRegistrationRepository_Tests : EventHubDomainTestBase
   {
       private readonly IEventRegistrationRepository _repository;
       private readonly EventHubTestData _testData;

       public EventRegistrationRepository_Tests()
       {
           _repository = GetRequiredService<IEventRegistrationRepository>();
           _testData = GetRequiredService<EventHubTestData>();
       }

       [Fact]
       public async Task Exists_Should_Return_False_If_Not_Registered()
       {
           var exists = await _repository.ExistsAsync(
               _testData.AbpMicroservicesFutureEventId,
               _testData.UserJohnId);
           exists.ShouldBeFalse();
       }
   }
   ```

3. **Testing Domain Services**
   ```csharp
   public class EventManager_Tests : EventHubDomainTestBase
   {
       [Fact]
       public async Task Should_Update_The_Event_Capacity()
       {
           const int newCapacity = 42;
           
           await WithUnitOfWorkAsync(async () =>
           {
               var @event = await _eventRepository.GetAsync(
                   _testData.AbpMicroservicesFutureEventId);
               await _eventManager.SetCapacityAsync(@event, newCapacity);
           });

           var @event = await _eventRepository.GetAsync(
               _testData.AbpMicroservicesFutureEventId);
           @event.Capacity.ShouldBe(newCapacity);
       }
   }
   ```

4. **Testing Application Services**
   ```csharp
   public class EventRegistrationAppService_Tests : EventHubApplicationTestBase
   {
       [Fact]
       public async Task Should_Register_To_An_Event()
       {
           Login(_testData.UserAdminId);
           
           await _eventRegistrationAppService.RegisterAsync(
               _testData.AbpMicroservicesFutureEventId
           );

           var registration = await GetRegistrationOrNull(
               _testData.AbpMicroservicesFutureEventId,
               _currentUser.GetId()
           );
           
           registration.ShouldNotBeNull();
       }

       private void Login(Guid userId)
       {
           _currentUser.Id.Returns(userId);
           _currentUser.IsAuthenticated.Returns(true);
       }
   }
   ```

### Best Practices

1. **Unit Testing**
   - Keep tests focused and isolated
   - Use proper mocking
   - Follow AAA pattern (Arrange-Act-Assert)
   - Use meaningful test names
   - Test both success and failure cases

2. **Integration Testing**
   - Use proper database mocking
   - Handle unit of work properly
   - Seed test data when needed
   - Test real-world scenarios
   - Consider performance implications

3. **Test Organization**
   - Follow consistent naming conventions
   - Group related tests
   - Use test base classes
   - Document complex test scenarios
   - Keep tests maintainable

4. **Test Data**
   - Use test data seeders
   - Keep test data realistic
   - Clean up test data properly
   - Consider data isolation
   - Document test data structure

5. **General Guidelines**
   - Write tests for critical paths
   - Keep tests simple and readable
   - Use appropriate test types
   - Consider test performance
   - Document test requirements

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards