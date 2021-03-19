# Configuration
```cs
interface IConfigurationProvider
{
    object GetConnectionConfiguration(string type, string name);

    FlowConfig GetFlow(string name);

    IObservable<(string Name, FlowConfig Config)> GetFlows();
}

class FlowConfig
{
    public BlockConfig Trigger { get; }

    public IReadOnlyList<BlockConfig> Actions { get; }
}

class BlockConfig
{
    public string Type { get; }

    public string ConfigurationName { get; }

    public IReadOnlyDictionary<string, object> Arguments { get; }
}
```

# Connectors
```cs
interface IConnector
{
    Type ConfigurationType { get; }

    IConnection GetConnection(object configuration);
}

interface IConnectorRegistry
{
    void Register(IConnector connector);

    void Unregister(IConnector connector);

    IConnector Get(string name);
}
```

# Connections
```cs
interface IConnection
{
    IFlowTrigger CreateTrigger(string name);

    IFlowAction CreateAction(string name);
}
```

# Parameters
```cs
interface IParameter<out T>
{
    Task<T> Resolve(FlowContext context);
}

class ConstantParameter<T> : IParameter<T>
{
    private readonly T value;

    public ConstantParameter(T value)
    {
        this.value = value;
    }

    Task<T> Resolve(FlowContext context) => Task.FromResult(value);
}

class VariableParameter<T> : IParameter<T>
{
    public VariableParameter(string path)
    {
        ...
    }

    Task<T> Resolve(FlowContext context) => // Resolve from context
}

class ExpressionParameter<T> : IParameter<T>
{
    public ExpressionParameter<T>(string expressionCode)
    {
        // Compile expression into script
    }

    Task<T> Resolve(FlowContext context) => // Resolve by running script
}
```

# Context
```cs
class FlowContext
{
    public Task AddData(string type, JObject data);

    public Task<JToken> GetData(string path);

    public Task<JObject> AsObject();

    public FlowActor Actor { get; }

    public FlowContext Clone();
}
```

# Trigger
```cs
interface IFlowTrigger
{
    string Type { get; }

    Type ArgumentsType { get; }

    IObservable<JObject> GetEvents(object arguments)
}
```

# Actions
```cs
interface IFlowAction
{
    string Type { get; }

    Type ArgumentsType { get; }

    Task Act(FlowContext context, object arguments);
}
```

# Flow
```cs
class FlowActions
{
    public IReadOnlyList<IFlowAction> Actions { get; }

    public Task Act(FlowContext context);
}

class Flow
{
    public string Name { get; }

    public IFlowTrigger Trigger { get; }

    public IReadOnlyList<IFlowAction> Actions { get; }
}

class FlowActor
{
    public Task Act(
        IReadOnlyList<BlockConfig> actions,
        FlowContext context);
}
```
