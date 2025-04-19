using easyshifthq.Samples;
using Xunit;

namespace easyshifthq.EntityFrameworkCore.Applications;

[Collection(easyshifthqTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<easyshifthqEntityFrameworkCoreTestModule>
{

}
