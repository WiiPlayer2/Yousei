using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    class JobAction
    {
        public string ModuleID { get; }

        public JToken Arguments { get; }
    }

    class Job
    {
        public string Name { get; }

        public IReadOnlyCollection<JobAction> Actions { get; }
    }
}
