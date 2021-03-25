using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class Api : IApi
    {
        private readonly IConfigurationDatabase configurationDatabase;

        private readonly EventHub eventHub;

        public Api(
            IConfigurationDatabase configurationDatabase,
            EventHub eventHub)
        {
            this.configurationDatabase = configurationDatabase;
            this.eventHub = eventHub;
        }

        public IConfigurationDatabase ConfigurationDatabase => configurationDatabase;

        public Task Reload()
        {
            eventHub.TriggerReload();
            return Task.CompletedTask;
        }
    }
}