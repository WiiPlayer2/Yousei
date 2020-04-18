using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    class JobRegistry
    {
        private readonly List<Job> jobs = new List<Job>();

        public IReadOnlyCollection<Job> Jobs => jobs;
    }
}
