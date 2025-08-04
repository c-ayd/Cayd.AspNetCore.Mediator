using Cayd.AspNetCore.Mediator.Abstraction;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Requests;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Handlers
{
    public class MultiplyHandler : IAsyncRequestHandler<MultiplyRequest, MultiplyResponse>
    {
        private readonly TestTransientService _testTransientService;
        private readonly TestScopedService _testScopedService;
        private readonly TestSingletonService _testSingletonService;

        public MultiplyHandler(TestTransientService testTransientService,
            TestScopedService testScopedService,
            TestSingletonService testSingletonService)
        {
            _testTransientService = testTransientService;
            _testScopedService = testScopedService;
            _testSingletonService = testSingletonService;
        }

        public async Task<MultiplyResponse> HandleAsync(MultiplyRequest request, CancellationToken cancellationToken = default)
        {
            ++_testTransientService.Counter;
            ++_testScopedService.Counter;
            ++_testSingletonService.Counter;

            return new MultiplyResponse()
            {
                Result = request.Value * request.Multiply,
                TransientCounter = _testTransientService.Counter,
                ScopedCounter = _testScopedService.Counter,
                SingletonCounter = _testSingletonService.Counter
            };
        }
    }
}
