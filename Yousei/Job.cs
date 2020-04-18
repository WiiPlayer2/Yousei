using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    class JobAction
    {
        public string ModuleID { get; set; }

        public JToken Arguments { get; set; }
    }

    class Job
    {
        public string Name { get; set; }

        public List<JobAction> Actions { get; set; }
    }
}
