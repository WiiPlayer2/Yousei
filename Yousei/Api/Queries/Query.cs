using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Queries
{
    public class Query
    {
        public Query(IApi api)
        {
            Database = new Database(api);
        }

        public Database Database { get; }
    }
}