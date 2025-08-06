using Cayd.AspNetCore.Mediator.DependencyInjection;
using Cayd.AspNetCore.Mediator.Test.Integration.OtherAssembly.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Flows;
using Cayd.AspNetCore.Mediator.Test.Utility.Requests;
using Cayd.AspNetCore.Mediator.Test.Utility.Services;
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

#if NET6_0_OR_GREATER
using System.Threading;
#endif

namespace Cayd.AspNetCore.Mediator.Test.Utility.Fixtures
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

                            services.AddScoped<OrderedService>();

                            services.AddMediator(config =>
                            {
                                config.AddHandlersFromAssembly(Assembly.GetAssembly(typeof(TestHostFixture))!);

                                config.AddTransientFlow(typeof(TestTransientFlow<,>));
                                config.AddScopedFlow(typeof(TestScopedFlow<,>));
                                config.AddSingletonFlow(typeof(TestSingletonFlow<,>));

                                config.AddTransientFlow(typeof(CancelFlow));

                                config.AddScopedFlow(typeof(Ordered1Flow));
                                config.AddTransientFlow(typeof(Ordered2Flow));
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
            endpoints.MapGet("/test/initialize", async (context) =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsync("Success");
            });

            endpoints.MapGet("/test/add", async (context) =>
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

            endpoints.MapGet("/test/substract", async (context) =>
            {
                var request = new SubstractDelayedRequest()
                {
                    Value = int.Parse(context.Request.Query["value"]!),
                    Substract = int.Parse(context.Request.Query["substract"]!)
                };

                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(request);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });

            endpoints.MapGet("/test/multiply", async (context) =>
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

            endpoints.MapGet("/test/divide", async (context) =>
            {
                var request = new DivideOtherAssemblyRequest()
                {
                    Value = int.Parse(context.Request.Query["value"]!),
                    Divide = int.Parse(context.Request.Query["divide"]!)
                };

                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(request);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });

#if NET6_0_OR_GREATER
            endpoints.MapGet("/test/cancel", async (HttpContext context, CancellationToken cancellationToken) =>
            {
                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(new CancelledRequest(), cancellationToken);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });
#endif

            endpoints.MapGet("/test/ordered", async (context) =>
            {
                var service = context.RequestServices.GetRequiredService<OrderedService>();
                service.Value = 20;

                var request = new OrderedRequest()
                {
                    Value = 9
                };

                var mediator = context.RequestServices.GetRequiredService<IMediator>();
                var response = await mediator.SendAsync(request);

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsJsonAsync(response);
            });

            endpoints.MapGet("/test/speed", async (context) =>
            {
                var isMediatorOn = bool.Parse(context.Request.Query["mediator"]!);
                if (isMediatorOn)
                {
                    var request = new AddRequest()
                    {
                        Value = 5,
                        Add = 10
                    };

                    var mediator = context.RequestServices.GetRequiredService<IMediator>();
                    var response = await mediator.SendAsync(request);
                }

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                await context.Response.WriteAsJsonAsync("Success");
            });
        }
    }
}
