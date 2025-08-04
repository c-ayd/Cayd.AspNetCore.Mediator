using Cayd.AspNetCore.Mediator.Test.Integration.Collections;
using Cayd.AspNetCore.Mediator.Test.Integration.Fixtures;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Responses;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration
{
    [Collection(nameof(TestHostCollection))]
    public partial class MediatorTest
    {
        private readonly TestHostFixture _testHostFixture;

        public MediatorTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
        }

        private const int _numberOfRepetingTest = 10;

        private const string _addRequestEndpoint = "/test/add-request/";
        private const string _multiplyRequestEndpoint = "/test/multiply-request/";

        [Fact]
        public async Task AddRequestEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var add = random.Next(0, 10);

                var currentSingletonCounter = _testHostFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostFixture.Client.GetAsync(_addRequestEndpoint + $"?value={value}&add={add}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);

                var response = await result.Content.ReadFromJsonAsync<AddResponse>(new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                Assert.NotNull(response);
                Assert.Equal(value + add, response.Result);
                Assert.Equal(1, response.TransientCounter);
                Assert.Equal(1, response.ScopedCounter);
                Assert.Equal(currentSingletonCounter + 1, response.SingletonCounter);
            }
        }

        [Fact]
        public async Task MultiplyRequestEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var multiply = random.Next(0, 10);

                var currentSingletonCounter = _testHostFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostFixture.Client.GetAsync(_multiplyRequestEndpoint + $"?value={value}&multiply={multiply}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);

                var response = await result.Content.ReadFromJsonAsync<MultiplyResponse>(new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                Assert.NotNull(response);
                Assert.Equal(value * multiply, response.Result);
                Assert.Equal(1, response.TransientCounter);
                Assert.Equal(1, response.ScopedCounter);
                Assert.Equal(currentSingletonCounter + 1, response.SingletonCounter);
            }
        }
    }
}
