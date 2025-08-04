using Cayd.AspNetCore.Mediator.Abstraction;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Requests;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Handlers
{
    public class AddHandler : IAsyncRequestHandler<AddRequest, AddResponse>
    {
        private readonly TestTransientService _testTransientService;
        private readonly TestScopedService _testScopedService;
        private readonly TestSingletonService _testSingletonService;

        public AddHandler(TestTransientService testTransientService,
            TestScopedService testScopedService,
            TestSingletonService testSingletonService)
        {
            _testTransientService = testTransientService;
            _testScopedService = testScopedService;
            _testSingletonService = testSingletonService;
        }

        public async Task<AddResponse> HandleAsync(AddRequest request, CancellationToken cancellationToken = default)
        {
            ++_testTransientService.Counter;
            ++_testScopedService.Counter;
            ++_testSingletonService.Counter;

            return new AddResponse()
            {
                Result = request.Value + request.Add,
                TransientCounter = _testTransientService.Counter,
                ScopedCounter = _testScopedService.Counter,
                SingletonCounter = _testSingletonService.Counter
            };
        }
    }
}
