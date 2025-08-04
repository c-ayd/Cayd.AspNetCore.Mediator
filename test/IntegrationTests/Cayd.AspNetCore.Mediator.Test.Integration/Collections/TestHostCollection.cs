using Cayd.AspNetCore.Mediator.Test.Integration.Fixtures;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Collections
{
    [CollectionDefinition(nameof(TestHostCollection))]
    public class TestHostCollection : ICollectionFixture<TestHostFixture>
    {
    }
}
