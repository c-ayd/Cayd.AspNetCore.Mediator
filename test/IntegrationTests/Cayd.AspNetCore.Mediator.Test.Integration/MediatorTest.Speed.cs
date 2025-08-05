using System;
using System.Threading.Tasks;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration
{
    public partial class MediatorTest
    {
        private const string _initializeEndpoint = "/test/initialize";
        private const string _speedEndpointMediatorOn = "/test/speed?mediator=true";
        private const string _speedEndpointMediatorOff = "/test/speed?mediator=false";

        [Fact]
        public async Task SpeedEndpoint_WhenRequestsAreMade_ShouldTakeLongerOnceAndThenFast()
        {
            // Arrange
            DateTime startTime;
            DateTime endTime;
            double totalTime;

            var coldStartResponse = await _testHostFixture.Client.GetAsync(_initializeEndpoint);
            if (!coldStartResponse.IsSuccessStatusCode)
                Assert.Fail("The cold start went wrong. Rerun the test.");

            // Act
            startTime = DateTime.UtcNow;
            var initialRequest = await _testHostFixture.Client.GetAsync(_speedEndpointMediatorOn);
            endTime = DateTime.UtcNow;

            if (!initialRequest.IsSuccessStatusCode)
                Assert.Fail("While executing the initial request, something went wrong. Rerun the test.");

            totalTime = 0.0;
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                startTime = DateTime.UtcNow;
                var response = await _testHostFixture.Client.GetAsync(_speedEndpointMediatorOn);
                endTime = DateTime.UtcNow;

                if (!response.IsSuccessStatusCode)
                    Assert.Fail("While executing the requests, something went wrong. Rerun the test.");

                totalTime += (endTime - startTime).TotalMilliseconds;
            }

            var averageTime = totalTime / _numberOfRepetingTest;

            totalTime = 0.0;
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                startTime = DateTime.UtcNow;
                var response = await _testHostFixture.Client.GetAsync(_speedEndpointMediatorOff);
                endTime = DateTime.UtcNow;

                if (!response.IsSuccessStatusCode)
                    Assert.Fail("While executing the reference requests, something went wrong. Rerun the test.");

                totalTime += (endTime - startTime).TotalMilliseconds;
            }

            var averageReferenceTime = totalTime / _numberOfRepetingTest;

            _testOutputHelper.WriteLine($"The initial request took {(endTime - startTime).TotalMilliseconds} ms.");
            _testOutputHelper.WriteLine($"The average time is {averageTime} ms.");
            _testOutputHelper.WriteLine($"The average reference time is {averageReferenceTime} ms.");
            _testOutputHelper.WriteLine($"The average difference is {averageTime - averageReferenceTime} ms.");
        }
    }
}
