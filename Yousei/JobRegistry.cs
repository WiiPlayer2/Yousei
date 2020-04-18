using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    class JobRegistry
    {
        private readonly List<Job> jobs = new List<Job>();

        public JobRegistry()
        {
            Register(new Job
            {
                Name = "testing",
                Actions = new List<JobAction>
                {
                    new JobAction
                    {
                        ModuleID = "shell",
                        Arguments = JToken.FromObject(new
                        {
                            Command = "echo hi",
                        }),
                    },
                },
            });
        }

        public IReadOnlyCollection<Job> Jobs => jobs;

        public void Register(Job job) => jobs.Add(job);
    }
}
