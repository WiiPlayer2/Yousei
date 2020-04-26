using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    public enum RestartPolicy
    {
        Never,
        Always,
        OnlyOnFailed,
    }

    public class JobAction
    {
        public string ModuleID { get; set; }

        public JToken Arguments { get; set; }
    }

    class Job
    {
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public RestartPolicy Restart { get; set; } = RestartPolicy.OnlyOnFailed;

        public List<JobAction> Actions { get; set; }

        public override string ToString() => $"{Name} ({GetHashCode():X8})";
    }
}
