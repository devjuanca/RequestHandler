# Easy Request Handler

RequestHandlers is a lightweight .NET library designed to simplify event and request handling. It integrates seamlessly with the .NET Dependency Injection (DI) system and supports asynchronous operations, making it ideal for building event-driven architectures and modular applications.

## Features

- **Event Handling**: Define and manage event handlers using the `IEventHandler<TEvent>` or `ITransactionalEventHandler<TEvent>` interfaces. Publish events to all registered handlers using the `IEventPublisher`.
- **Request Handling**: Create and manage request handlers using the `RequestHandler<TRequest, TResponse>` and `RequestHandlers<TResponse>` base classes.
- **Dependency Injection**: Automatically register your event and request handlers with the built-in extension methods for `IServiceCollection`.
- **Customizable Service Lifetimes**: Use the `HandlerLifetimeAttribute` to specify the desired service lifetime (Singleton, Scoped, Transient) for your handlers.
- **Asynchronous Operations**: Leverage async/await patterns for non-blocking event and request processing.
- **Built-in Logging**: Integrated error handling and logging provide insights into the execution flow.

## Installation

To install RequestHandlers, run the following command in your NuGet Package Manager Console:

```sh
Install-Package EasyRequestHandler
```

## Usage
1. Defining Event Handlers

To define an event handler, implement the IEventHandler<TEvent> interface in your class:

``` csharp
public class MyEvent
{
    public string Message { get; set; }
}

public class MyEventHandler : IEventHandler<MyEvent>
{
    public Task HandleAsync(MyEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine(@event.Message);
        return Task.CompletedTask;
    }
}
```

3. Defining Event Transactional Handlers

To define an transactional event handler, use the `ITransactionalEventHandler<TEvent>` interface

``` csharp
public class MyEvent
{
    public string Message { get; set; }
}

public class MyEventHandler : ITransactionalEventHandler<MyEvent>
{
    public int Order { get; } = 0;

    public Task HandleAsync(MyEvent @event, CancellationToken cancellationToken)
    {
        Console.WriteLine(@event.Message);
        return Task.CompletedTask;
    }

    public Task CommitAsync(MyEvent @event, CancellationToken cancellationToken)
    {
        // Add logic to persist or commit changes here
        return Task.CompletedTask;
    }

    public Task RollbackAsync(MyEvent @event, CancellationToken cancellationToken)
    {
        // Add logic to undo changes here
        return Task.CompletedTask;
    }
}
```

Note: Transactional Events will be executed ordered by the Order property. 

An event can have both transactional and non-transactional handlers.

4. Defining Request Handlers

To define a request handler, inherit from `RequestHandler<TRequest, TResponse>` or `RequestHandler<TResponse>`:

```csharp
public class MyRequest
{
    public int Number { get; set; }
}

public class MyResponse
{
    public int Result { get; set; }
}

public class MyRequestHandler : RequestHandlers<MyRequest, MyResponse>
{
    public override ValueTask<MyResponse> HandleAsync(MyRequest request, CancellationToken cancellationToken = default)
    {
        var response = new MyResponse { Result = request.Number * 2 };
        return new ValueTask<MyResponse>(response);
    }
}

```

5. Registering Handlers with Dependency Injection

To register your event and request handlers with the DI container, use the provided extension methods:

```csharp

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Register event handlers
        services.AddEventsHandlers(typeof(MyEventHandler).Assembly);

        // Register request handlers
        services.AddRequestsHandlers(typeof(MyRequestHandler).Assembly);
    }
}

```

6. Configuring Service Lifetimes

Use the HandlerLifetimeAttribute to configure the lifetime of your handlers (both types events or requests):

```csharp
[HandlerLifetime(ServiceLifetime.Singleton)]
public class MySingletonEventHandler : IEventHandler<MyEvent>
{
    // Implementation
}

```

## Sample Project

For more detailed examples and advanced usage, check out the sample project included in the repository. It demonstrates various scenarios and best practices for using the RequestHandlers library in real-world applications.
