using Xunit;

namespace easyshifthq.EntityFrameworkCore;

[CollectionDefinition(easyshifthqTestConsts.CollectionDefinitionName)]
public class easyshifthqEntityFrameworkCoreCollection : ICollectionFixture<easyshifthqEntityFrameworkCoreFixture>
{

}
