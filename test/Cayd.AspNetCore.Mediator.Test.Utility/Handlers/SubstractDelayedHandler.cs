using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Handlers
{
    public class SubstractDelayedHandler : IAsyncHandler<SubstractDelayedRequest, SubstractDelayedResponse>
    {
        private readonly TestTransientService _testTransientService;
        private readonly TestScopedService _testScopedService;
        private readonly TestSingletonService _testSingletonService;

        public SubstractDelayedHandler(TestTransientService testTransientService,
            TestScopedService testScopedService,
            TestSingletonService testSingletonService)
        {
            _testTransientService = testTransientService;
            _testScopedService = testScopedService;
            _testSingletonService = testSingletonService;
        }

        public async Task<SubstractDelayedResponse> HandleAsync(SubstractDelayedRequest request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);

            ++_testTransientService.Counter;
            ++_testScopedService.Counter;
            ++_testSingletonService.Counter;

            return new SubstractDelayedResponse()
            {
                Result = request.Value - request.Substract,
                TransientCounter = _testTransientService.Counter,
                ScopedCounter = _testScopedService.Counter,
                SingletonCounter = _testSingletonService.Counter
            };
        }
    }
}
