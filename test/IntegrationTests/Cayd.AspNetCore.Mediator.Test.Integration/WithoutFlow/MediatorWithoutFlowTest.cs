using Cayd.AspNetCore.Mediator.Test.Integration.Collections;
using Cayd.AspNetCore.Mediator.Test.Utility.Fixtures;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
using Cayd.AspNetCore.Mediator.Test.Utility.Responses;
using System.Net.Http.Json;
using Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Responses;
using Xunit.Abstractions;

namespace Cayd.AspNetCore.Mediator.Test.Integration.WithoutFlow
{
    [Collection(nameof(TestHostWithoutFlowCollection))]
    public partial class MediatorWithoutFlowTest
    {
        private const int _numberOfRepetingTest = 10;

        private const string _addRequestEndpoint = "/test/add";
        private const string _substractRequestEndpoint = "/test/substract";
        private const string _multiplyRequestEndpoint = "/test/multiply";
        private const string _divideRequestEndpoint = "/test/divide";

        private readonly TestHostWithoutFlowFixture _testHostWithoutFlowFixture;

        private readonly ITestOutputHelper _testOutputHelper;

        public MediatorWithoutFlowTest(TestHostWithoutFlowFixture testHostWithoutFlowFixture,
            ITestOutputHelper testOutputHelper)
        {
            _testHostWithoutFlowFixture = testHostWithoutFlowFixture;
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task AddEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var add = random.Next(0, 10);

                var currentSingletonCounter = _testHostWithoutFlowFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostWithoutFlowFixture.Client.GetAsync(_addRequestEndpoint + $"?value={value}&add={add}");

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
        public async Task SubstractEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var substract = random.Next(0, 10);

                var currentSingletonCounter = _testHostWithoutFlowFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostWithoutFlowFixture.Client.GetAsync(_substractRequestEndpoint + $"?value={value}&substract={substract}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);

                var response = await result.Content.ReadFromJsonAsync<SubstractDelayedResponse>(new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                Assert.NotNull(response);
                Assert.Equal(value - substract, response.Result);
                Assert.Equal(1, response.TransientCounter);
                Assert.Equal(1, response.ScopedCounter);
                Assert.Equal(currentSingletonCounter + 1, response.SingletonCounter);
            }
        }

        [Fact]
        public async Task MultiplyEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var multiply = random.Next(0, 10);

                var currentSingletonCounter = _testHostWithoutFlowFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostWithoutFlowFixture.Client.GetAsync(_multiplyRequestEndpoint + $"?value={value}&multiply={multiply}");

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

        [Fact]
        public async Task DivideEndpoint_WhenRequestExists_ShouldCallCorrespondingHandler()
        {
            var random = new Random();
            for (int i = 0; i < _numberOfRepetingTest; ++i)
            {
                // Arrange
                var value = random.Next(0, 10);
                var divide = random.Next(1, 10);

                var currentSingletonCounter = _testHostWithoutFlowFixture.Host.Services.GetRequiredService<TestSingletonService>().Counter;

                // Act
                var result = await _testHostWithoutFlowFixture.Client.GetAsync(_divideRequestEndpoint + $"?value={value}&divide={divide}");

                // Assert
                Assert.Equal(HttpStatusCode.OK, result.StatusCode);

                var response = await result.Content.ReadFromJsonAsync<DivideOtherAssemblyResponse>(new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });

                Assert.NotNull(response);
                Assert.Equal(value / divide, response.Result);
            }
        }
    }
}
