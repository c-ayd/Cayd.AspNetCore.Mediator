## About
This is a mediator library that handles request-handler-response flows for ASP.NET Core. It supports automatically matching requests with corresponding handlers as well as custom flows for all requets and/or specific ones.

## How to Use
After installing the package, you can set up your own request-handler-response flow by using `IAsyncRequest<TResponse>` and `IAsyncHandler<TRequest, TResponse>`. Afterwards, you need to add the mediator to the dependency injection. So that, you can use the `SendAsync` method of the `IMediator` interface.

- Creating Request-Handler-Response Flow:
```csharp
using Cayd.AspNetCore.Mediator.Abstractions;

public class MyRequest : IAsyncRequest<MyResponse>
{
    // ... properties of the request
}

public class MyHandler : IAsyncHandler<MyRequest, MyResponse>
{
    // Example dependency injection
    private readonly ILogger<MyHandler> _logger;

    // You can inject dependencies if needed
    public MyHandler(ILogger<MyHandler> logger)
    {
        _logger = logger;  // example
    }

    public async Task<MyResponse> HandleAsync(MyRequest request, CancellationToken cancellationToken)
    {
        // ... handle the request and return the corresponding response
    }
}

public class MyResponse
{
    // ... properties of the response
}
```
- Adding Mediator and Handlers:
```csharp
/**
 * Minimal API - Program.cs
 */
using Cayd.AspNetCore.Mediator.DependencyInjection;

builder.Services.AddMediator(config =>
{
    // Add handlers from one assembly
    config.AddHandlersFromAssembly(Assembly.GetAssembly(typeof(MyClassInAssembly)!));

    // ... or from multiple assemblies
    config.AddHandlersFromAssemblies(Assembly.GetAssembly(typeof(MyClassInAssembly)!,
        typeof(MyClassInAnotherAssembly)!));
});


/**
 * .NET 5 - Startup.cs
 */
using Cayd.AspNetCore.Mediator.DependencyInjection;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMediator(config =>
        {
            // Add handlers from one assembly
            config.AddHandlersFromAssembly(Assembly.GetAssembly(typeof(MyClassInAssembly)!));

            // ... or from multiple assemblies
            config.AddHandlersFromAssemblies(Assembly.GetAssembly(typeof(MyClassInAssembly)!,
                typeof(MyClassInAnotherAssembly)!));
        });
    }
}
```
- Sending Request:
```csharp
using Cayd.AspNetCore.Mediator;

[ApiController]
public class MyController : ControllerBase
{
    private readonly IMediator _mediator;

    public MyController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("example")]
    public async Task<IActionResult> Example()
    {
        var response = await _mediator.SendAsync(new MyRequest()
        {
            // ... properties of the request
        });

        // ... handle response
    }

    [HttpGet("example-with-cancellation")]
    public async Task<IActionResult> Example(CancellationToken cancellationToken)
    {
        var response = await _mediator.SendAsync(new MyRequest()
        {
            // ... properties of the request
        }, cancellationToken);

        // ... handle response
    }
}
```

## Custom Flow
You can customize the default request-handler-response flow structure by adding your own custom flow elements for all requests and/or specific ones. To do that, you need to create a class implementing the `IMediatorFlow<TRequest, TResponse>` interface and add it via the dependency injection extension methods of the library.

- Custom Flow Element For All Requets:

If you create a generic `IMediatorFlow` like below, this flow element is applied to all requests.
```csharp
using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;

public class MyCustomMediatorFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
    where TRequest : IAsyncRequest<TResponse>
{
    // Example dependency injection
    private readonly ILogger<MyCustomMediatorFlow<TRequest, TResponse>> _logger;

    // You can inject dependencies if needed
    public MyCustomMediatorFlow(ILogger<MyCustomMediatorFlow<TRequest, TResponse>> logger)
    {
        _logger = logger;   // example
    }

    public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // ... custom logic

        // Call the next one
        return await next();
    }
}
```
- Custom Flow Element For Specific Request:

If you specify the request and response types while creating `IMediatorFlow`, the flow element is applied only to that specific request.
```csharp
using Cayd.AspNetCore.Mediator.Flows;

public class MyCustomSpecificMediatorFlow : IMediatorFlow<MyRequest, MyResponse>
{
    // Example dependency injection
    private readonly ILogger<MyCustomSpecificMediatorFlow> _logger;

    // You can inject dependencies if needed
    public MyCustomMediatorFlow(ILogger<MyCustomSpecificMediatorFlow> logger)
    {
        _logger = logger;   // example
    }

    public async Task<MyResponse> InvokeAsync(MyRequest request, AsyncHandlerDelegate<MyResponse> next, CancellationToken cancellationToken)
    {
        // ... custom logic

        // Call the next one
        return await next();
    }
}
```
- Adding Custom Flows:
```csharp
services.AddMediator(config =>
{
    // ... add handlers

    config.AddTransientFlow(typeof(MyCustomMediatorFlow<,>));        // for all requests
    config.AddTransientFlow(typeof(MyCustomSpecificMediatorFlow));   // only for 'MyRequest'
});
```

`NOTE`: The order of added custom flow elements defines which one is invoked first.

## Performance
- The library has two different behaviors based on if any custom flow is added. If there is no custom flow, the request-handler-response flow skips looking for custom flows, which speeds up the process.
- When a `IAsyncRequest<TResponse>` is sent, the library needs to generate a delegate to cache the related flow for the first time. This initial process adds a negligible overhead (less than `1ms` according to the local tests) to the request, which also varies based on if there is any custom flow. Afterwards, when the same request is sent, the library uses the cached delegate to speed up the process.

## Extras
For CQRS handlers utilizing the `Cayd.AspNetCore.ExecResult` and `FluentValidation` libraries, the validation flow can be set up as follows to use `ExecBadRequest` automatically when validations fail:
- Validation Flow:
```csharp
using Cayd.AspNetCore.ExecutionResult;
using Cayd.AspNetCore.ExecutionResult.ClientError;
using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Flows;
using FluentValidation;

public class MediatorValidationFlow<TRequest, TResponse> : IMediatorFlow<TRequest, TResponse>
    where TRequest : IAsyncRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public MediatorValidationFlow(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> InvokeAsync(TRequest request, AsyncHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var validator in _validators)
        {
            if (validator == null)
                continue;

            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (validationResult.Errors.Count > 0)
            {
                var errorDetails = validationResult.Errors
                    .Select(e => new ExecErrorDetail(e.ErrorMessage, e.ErrorCode))
                    .ToList();

                return (dynamic)new ExecBadRequest(errorDetails);
            }
        }

        return await next();
    }
}
```
- Example Validation in CQRS:
```csharp
public class MyRequestValidation : AbstractValidator<MyRequest>
{
    public LoginValidation()
    {
        RuleFor(r => r.Property1)
            .NotEmpty()
                .WithMessage("Error message here")
                .WithErrorCode("Error message code here (can be used for translation keys for instance)");
    }
}
```
- Adding The Validation Flow:
```csharp
services.AddMediator(config =>
{
    // ...

    config.AddTransientFlow(typeof(MediatorValidationFlow<,>));
});
```
