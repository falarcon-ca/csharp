// DEPENDENCY INJECTION .NET: SERVICE LIFETIME
// This is an implementation of DI Container in Console app to check the service lifetime in net 6
// Singleton: always the same instance
// Scoped: change instance when change the scope
// Transient: always a different instance

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
        services.AddSingleton<ISingletonOperation, DefaultOperation>()
        .AddScoped<IScopedOperation, DefaultOperation>()
        .AddTransient<ITransientOperation, DefaultOperation>()
        .AddTransient<OperationLogger>()
).Build();

ExecuteLifeTime(host.Services, "Scope 1");
ExecuteLifeTime(host.Services, "Scope 2");

await host.RunAsync();

static void ExecuteLifeTime(IServiceProvider services, string scope)
{
    using IServiceScope serviceScope = services.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    OperationLogger logger = provider.GetRequiredService<OperationLogger>();
    logger.LogOperations($"{scope}-Call 1.");

    Console.WriteLine("...");

    logger = provider.GetRequiredService<OperationLogger>();
    logger.LogOperations($"{scope}-Call 2.");

    Console.WriteLine();
}

// Interaces
public interface IOperation
{
    string InstanceId { get; }
}

public interface ISingletonOperation : IOperation { }

public interface IScopedOperation : IOperation { }

public interface ITransientOperation : IOperation { }

// Default implementation
public class DefaultOperation : ISingletonOperation, IScopedOperation, ITransientOperation
{
    public string InstanceId { get; } = Guid.NewGuid().ToString()[^4..];
}

// Logger implementation
public class OperationLogger
{
    private readonly ISingletonOperation singletonOperation;
    private readonly IScopedOperation scopedOperation;
    private readonly ITransientOperation transientOperation;
    public OperationLogger(ISingletonOperation singletonOperation, IScopedOperation scopedOperation, ITransientOperation transientOperation)
    {
        this.singletonOperation = singletonOperation;
        this.scopedOperation = scopedOperation;
        this.transientOperation = transientOperation;
    }

    public void LogOperations(string scope)
    {
        LogOperation(this.singletonOperation, scope);
        LogOperation(this.scopedOperation, scope);
        LogOperation(this.transientOperation, scope);
    }

    private void LogOperation<T>(T operation, string scope) where T : IOperation
    {
        Console.WriteLine($"{scope}: {typeof(T).Name,-19} [ {operation.InstanceId} ]");
    }
}
