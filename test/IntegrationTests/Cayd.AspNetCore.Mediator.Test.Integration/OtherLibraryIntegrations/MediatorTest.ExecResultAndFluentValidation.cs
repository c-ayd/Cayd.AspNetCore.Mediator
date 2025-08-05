using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.Mediator.DependencyInjection;
using Cayd.AspNetCore.Mediator.Test.Utility.Fixtures;
using Cayd.AspNetCore.Mediator.Test.Utility.OtherLibraryIntegrations.ExecResultAndFluentValidation;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration.OtherLibraryIntegrations
{
    public class MediatorTest_ExecResultAndFluentValidation : IDisposable
    {
        private const string _endpoint = "/test/exec-result-and-fluent-validation";

        private readonly IHost _host;
        private readonly HttpClient _client;

        public MediatorTest_ExecResultAndFluentValidation()
        {
            _host = new HostBuilder()
                .ConfigureWebHost(hostBuilder =>
                {
                    hostBuilder.UseTestServer()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddRouting();

                            services.AddScoped<IValidator<ExecResult_FluentValidation_Request>, ExecResult_FluentValidation_Validation>();

                            services.AddMediator(config =>
                            {
                                config.RegisterHandlersFromAssembly(Assembly.GetAssembly(typeof(TestHostFixture))!);

                                config.AddScopedFlow(typeof(ExecResult_FluentValidation_Flow<,>));
                            });
                        })
                        .Configure(appBuilder =>
                        {
                            appBuilder.UseRouting();
                            appBuilder.UseEndpoints(endpoints =>
                            {
                                endpoints.MapGet(_endpoint, async (context) =>
                                {
                                    var divide = int.Parse(context.Request.Query["divide"]!);
                                    var request = new ExecResult_FluentValidation_Request()
                                    {
                                        Divide = divide
                                    };

                                    var mediator = context.RequestServices.GetRequiredService<IMediator>();
                                    var response = await mediator.SendAsync(request);

                                    var objectResult = response.Match(
                                        (code, result, metadata) => Success(code, result, metadata),
                                        (code, errors, metadata) => Fail(code, errors, metadata)
                                    );

                                    context.Response.StatusCode = objectResult.StatusCode!.Value;
                                    context.Response.ContentType = MediaTypeNames.Application.Json;
                                    await context.Response.WriteAsJsonAsync(objectResult.Value);
                                });
                            });
                        });
                })
                .Build();

            _host.Start();
            _client = _host.GetTestClient();
        }

        private ObjectResult Success(int statusCode, object? data, object? metadata)
        {
            var body = new Dictionary<string, object?>()
            {
                { "statusCode", statusCode },
                { "data", data },
                { "metadata", metadata }
            };

            return new ObjectResult(body)
            {
                StatusCode = statusCode
            };
        }

        private ObjectResult Fail(int statusCode, object? errors, object? metadata)
        {
            var body = new Dictionary<string, object?>()
            {
                { "statusCode", statusCode },
                { "errors", errors },
                { "metadata", metadata }
            };

            return new ObjectResult(body)
            {
                StatusCode = statusCode
            };
        }

        public void Dispose()
        {
            _host.Dispose();
            _client.Dispose();
        }

        [Fact]
        public async Task Integration_WhenRequestIsValid_ShouldReturnOkAndResult()
        {
            // Act
            var result = await _client.GetAsync(_endpoint + "?divide=5");

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var json = await result.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(json);

            var statusCodeElement = jsonDocument.RootElement.GetProperty("statusCode");
            var statusCodeValue = JsonSerializer.Deserialize<int>(statusCodeElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Equal(200, statusCodeValue);

            var dataElement = jsonDocument.RootElement.GetProperty("data");
            var dataValue = JsonSerializer.Deserialize<ExecResult_FluentValidation_Response>(dataElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(dataValue);
            Assert.Equal(2, dataValue.Result);

            var metadataElement = jsonDocument.RootElement.GetProperty("metadata");
            var metadataValue = JsonSerializer.Deserialize<object>(metadataElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Null(metadataValue);
        }

        [Fact]
        public async Task Integration_WhenRequestIsInvalid_ShouldReturnBadRequestAndErrors()
        {
            // Act
            var result = await _client.GetAsync(_endpoint + "?divide=0");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);

            var json = await result.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(json);

            var statusCodeElement = jsonDocument.RootElement.GetProperty("statusCode");
            var statusCodeValue = JsonSerializer.Deserialize<int>(statusCodeElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Equal(400, statusCodeValue);

            var errorsElement = jsonDocument.RootElement.GetProperty("errors");
            var errorsValue = JsonSerializer.Deserialize<ICollection<ExecErrorDetail>>(errorsElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.NotNull(errorsValue);
            Assert.Single(errorsValue);
            Assert.Equal("test error message", errorsValue.ElementAt(0).Message);
            Assert.Equal("test error code", errorsValue.ElementAt(0).MessageCode);

            var metadataElement = jsonDocument.RootElement.GetProperty("metadata");
            var metadataValue = JsonSerializer.Deserialize<object>(metadataElement.GetRawText(), new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            Assert.Null(metadataValue);
        }
    }
}
