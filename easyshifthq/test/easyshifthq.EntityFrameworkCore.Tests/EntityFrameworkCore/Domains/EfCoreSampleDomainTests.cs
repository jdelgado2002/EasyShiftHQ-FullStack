using easyshifthq.Samples;
using Xunit;

namespace easyshifthq.EntityFrameworkCore.Domains;

[Collection(easyshifthqTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<easyshifthqEntityFrameworkCoreTestModule>
{

}
