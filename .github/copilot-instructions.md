# EasyShiftHQ Development Guidelines

## 1. Project Structure
- Solution is organized into multiple projects:
  - `easyshifthq.Domain`: Core domain logic
  - `easyshifthq.EntityFrameworkCore`: Data access layer
  - `easyshifthq.HttpApi`: API endpoints
  - `easyshifthq.Web`: Web interface
  - `easyshifthq.DbMigrator`: Database migration tool

## 2. Database Management
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
````typescript
// filepath: easyshifthq.Web/Components/EmployeeList.tsx
import React from 'react';
import { Table } from 'antd';
import { useEmployees } from '../hooks/useEmployees';

export const EmployeeList: React.FC = () => {
    const { data, loading } = useEmployees();

    return (
        <Table
            loading={loading}
            dataSource={data?.items}
            columns={[
                { title: 'First Name', dataIndex: 'firstName' },
                { title: 'Last Name', dataIndex: 'lastName' },
                { title: 'Email', dataIndex: 'email' }
            ]}
        />
    );
};
````

## 3. Testing Organization
- Maintain separate test projects:
  - `easyshifthq.EntityFrameworkCore.Tests`
  - `easyshifthq.Web.Tests`
- Write integration tests for data access
- Include UI component tests

### Test Structure Pattern
When creating tests that need to store or access data:

1. Create an abstract base test class with the module type parameter:
```csharp
public abstract class MyFeatureTests<TStartupModule> : easyshifthqApplicationTestBase<TStartupModule> 
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

## 4. Code Analysis
- Follow Microsoft Code Analysis rules
- Use provided `.editorconfig` settings
- Maintain consistency across:
  - Design rules
  - Documentation rules
  - Performance rules
  - Localization rules

## 5. API Development
- Implement RESTful endpoints in HttpApi project
- Use DTOs for data transfer
- Follow API versioning conventions
- Document endpoints with OpenAPI/Swagger

## 6. Security
- Implement multi-tenancy where required
- Use proper authentication/authorization
- Follow secure coding practices
- Validate all inputs

## 7. Frontend Development
- Organize web assets in `wwwroot`
- Use modern JavaScript libraries (e.g., Luxon for date handling)
- Implement responsive design
- Follow component-based architecture

## 8. Error Handling
- Implement proper exception handling
- Use custom exception types
- Log errors appropriately
- Return consistent error responses

## 9. Performance
- Follow performance best practices
- Optimize database queries
- Implement caching where appropriate
- Monitor application metrics

## 10. Documentation
- Maintain up-to-date API documentation
- Document complex business logic
- Include setup instructions
- Document configuration options

## 11. Development Workflow
- Follow Git branching strategy
- Use pull requests for code reviews
- Run tests before merging
- Keep dependencies updated

## 12. Configuration Management
- Use proper environment-specific settings
- Secure sensitive configuration
- Document configuration options
- Follow configuration best practices

---

For additional details, refer to:
- [ABP.io Documentation](https://docs.abp.io/)
- Project wiki
- Internal coding standards