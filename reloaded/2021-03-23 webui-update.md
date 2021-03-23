# Configuration Database
```cs
interface IConfigurationDatabase
{
    bool IsReadOnly { get; }

    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> ListConfigurations();
    Task<object> ReadConfiguration(string connector, string name);
    Task SetConfiguration(string connector, string name, object configuration);

    Task<IReadOnlyList<string>> ListFlows();
    Task<FlowConfig> ReadFlow(string name);
    Task SetFlow(string name, FlowConfig flowConfig);
}
```

# Webinterface API
```cs
interface IApi
{
    IConfigurationDatabase ConfigurationDatabase { get; }

    Task Reload();
}
```
First try to implement with GraphQL. If that does prove to be a failure, use REST.

# Webinterface UI
Vertical-tabbed experience based on monaco editor showing configuration in yaml representation.

# Internal
Enable reloading of connectors and flows to avoid memory leaks and zombie flows.
