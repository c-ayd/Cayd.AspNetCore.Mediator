using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Cayd.AspNetCore.Mediator.Test.Utility.Handlers
{
    public class AddHandler : IAsyncHandler<AddRequest, AddResponse>
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

        public async Task<AddResponse> HandleAsync(AddRequest request, CancellationToken cancellationToken)
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
