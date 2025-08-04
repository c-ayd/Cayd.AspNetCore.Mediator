using Cayd.AspNetCore.Mediator.DependencyInjection;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Requests;
using Cayd.AspNetCore.Mediator.Test.Integration.Utilities.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Cayd.AspNetCore.Mediator.Test.Integration.Fixtures
{
    public class TestHostFixture : IAsyncLifetime
    {
        public IHost Host { get; private set; } = null!;
        public HttpClient Client { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Host = new HostBuilder()
                .ConfigureWebHost(hostBuilder =>
                {
                    hostBuilder.UseTestServer()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddRouting();

                            services.AddTransient<TestTransientService>();
                            services.AddScoped<TestScopedService>();
                            services.AddSingleton<TestSingletonService>();

                            services.AddMediator(config =>
                            {
                                config.RegisterHandlersFromAssembly(Assembly.GetAssembly(typeof(TestHostFixture))!);
                            });
                        })
                        .Configure(appBuilder =>
                        {
                            appBuilder.UseRouting();
                            appBuilder.UseEndpoints(endpoints =>
                            {
                                AddEndpoints(endpoints);
                            });
                        });
                })
                .Build();

            await Host.StartAsync();
            Client = Host.GetTestClient();
        }

        public async Task DisposeAsync()
        {
            await Host.StopAsync();
            Host.Dispose();
            Client.Dispose();
        }

        private void AddEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGet("/initialize", async (context) =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsync("Success");
            });

            endpoints.MapGet("/test/add-request", async (context) =>
            {
                var request = new AddRequest()
                {
                    Value = int.Parse(context.Request.Query["value"]!),
                    Add = int.Parse(context.Request.Query["add"]!)
                };

                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(request);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });

            endpoints.MapGet("/test/multiply-request", async (context) =>
            {
                var request = new MultiplyRequest()
                {
                    Value = int.Parse(context.Request.Query["value"]!),
                    Multiply = int.Parse(context.Request.Query["multiply"]!)
                };

                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(request);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });
        }
    }
}
