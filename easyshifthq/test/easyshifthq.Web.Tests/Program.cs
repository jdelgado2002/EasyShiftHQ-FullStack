using Microsoft.AspNetCore.Builder;
using easyshifthq;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("easyshifthq.Web.csproj"); 
await builder.RunAbpModuleAsync<easyshifthqWebTestModule>(applicationName: "easyshifthq.Web");

public partial class Program
{
}
