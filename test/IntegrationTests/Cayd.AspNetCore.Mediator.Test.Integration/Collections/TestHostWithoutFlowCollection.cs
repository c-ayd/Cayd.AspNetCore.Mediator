using Cayd.AspNetCore.Mediator.Test.Utility.Fixtures;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Collections
{
    [CollectionDefinition(nameof(TestHostWithoutFlowCollection))]
    public class TestHostWithoutFlowCollection : ICollectionFixture<TestHostWithoutFlowFixture>
    {
    }
}
