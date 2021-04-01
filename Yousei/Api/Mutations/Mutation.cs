using HotChocolate;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Mutations
{
    public class Mutation
    {
        public async Task<Unit> Reload(
            [Service] IApi api)
        {
            await api.Reload();
            return Unit.Default;
        }
    }
}