using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace easyshifthq.Pages;

[Collection(easyshifthqTestConsts.CollectionDefinitionName)]
public class Index_Tests : easyshifthqWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
