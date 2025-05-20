# Easy Request Handler

RequestHandlers es una librería ligera para .NET que simplifica el manejo de eventos y requests. Integra fácilmente con el sistema de Inyección de Dependencias (DI) de .NET y soporta operaciones asíncronas, ideal para arquitecturas orientadas a eventos y aplicaciones modulares.

## Características

- **Manejo de eventos**: Define y gestiona handlers usando `IEventHandler<TEvent>` o `ITransactionalEventHandler<TEvent>`. Publica eventos a todos los handlers registrados con `IEventPublisher`.
- **Manejo de requests**: Crea y gestiona handlers usando las clases base `RequestHandler<TRequest, TResponse>` y `RequestHandlers<TResponse>`.
- **Inyección de dependencias**: Registra automáticamente tus handlers de eventos y requests con métodos de extensión para `IServiceCollection`.
- **Ciclo de vida configurable**: Usa `HandlerLifetimeAttribute` para especificar el ciclo de vida del servicio (Singleton, Scoped, Transient).
- **Operaciones asíncronas**: Soporta async/await para procesamiento no bloqueante.
- **Logging integrado**: Manejo de errores y logging para trazabilidad.
- **Patrón Mediador**: Soporte para pipeline behaviours y request hooks usando el builder pattern.

## Instalación

Para instalar RequestHandlers, ejecuta en tu consola de NuGet:

```sh
Install-Package EasyRequestHandler
```

## Uso

### 1. Definir Event Handlers

```csharp
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

### 2. Definir Transactional Event Handlers

```csharp
public class MyEventHandler : ITransactionalEventHandler<MyEvent>
{
    public int Order { get; } = 0;
    public Task HandleAsync(MyEvent @event, CancellationToken cancellationToken) { /* ... */ }
    public Task CommitAsync(MyEvent @event, CancellationToken cancellationToken) { /* ... */ }
    public Task RollbackAsync(MyEvent @event, CancellationToken cancellationToken) { /* ... */ }
}
```

> Los handlers transaccionales se ejecutan en orden según la propiedad `Order`.

### 3. Definir Request Handlers

```csharp
public class MyRequestHandler : RequestHandlers<MyRequest, MyResponse>
{
    public override ValueTask<MyResponse> HandleAsync(MyRequest request, CancellationToken cancellationToken = default)
    {
        var response = new MyResponse { Result = request.Number * 2 };
        return new ValueTask<MyResponse>(response);
    }
}
```

### 4. Registro de Handlers usando el Builder Pattern y Mediator

A partir de la versión más reciente, puedes registrar tus handlers y configurar el pipeline usando el builder pattern, habilitando el patrón mediador y behaviours:

```csharp
builder.Services.AddEasyRequestHandlers(typeof(Program))
    .WithMediatorPattern()
    .WithBehaviours(
        typeof(LoggingBehaviour<,>),
        typeof(AuthenticationBehaviour<,>),
        typeof(ValidationBehaviour<,>)
    )
    .WithRequestHooks()
    .Build();

builder.Services.AddEasyEventHandlers(typeof(Program));
```


Use Control + Shift + m to toggle the tab key moving focus. Alternatively, use esc then tab to move to the next interactive element on the page.
No file chosen
Attach files by dragging & dropping, selecting or pasting them.
Editing RequestHandler/README.md at master · devjuanca/RequestHandler
