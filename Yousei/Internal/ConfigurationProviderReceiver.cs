using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    public class ConfigurationProviderReceiver : IConfigurationProvider
    {
        private readonly IConfigurationDatabase database;

        private readonly IConfigurationProviderNotifier notifier;

        public ConfigurationProviderReceiver(IConfigurationProviderNotifier notifier, IConfigurationDatabase database)
        {
            this.notifier = notifier;
            this.database = database;
        }

        public object GetConnectionConfiguration(string type, string name)
            => database.GetConfiguration(type, name);

        public IObservable<(string Connector, string Name, object Configuration)> GetConnectionConfigurations()
            => Observable.DeferAsync(async _ =>
            {
                var configurations = await database.ListConfigurations();
                var tasks = configurations.SelectMany(o => o.Value, async (connector, name) => (connector.Key, name, await database.GetConfiguration(connector.Key, name)));
                return (await Task.WhenAll(tasks)).ToObservable();
            });

        public FlowConfig GetFlow(string name)
            => database.GetFlow(name).GetAwaiter().GetResult();

        public IObservable<(string Name, FlowConfig Config)> GetFlows()
            => Observable.DeferAsync(async _ =>
            {
                var flows = await database.ListFlows();
                var tasks = flows.Select(async o => (o, await database.GetFlow(o)));
                return (await Task.WhenAll(tasks)).ToObservable();
            })
            .Concat(notifier.Flows);
    }
}