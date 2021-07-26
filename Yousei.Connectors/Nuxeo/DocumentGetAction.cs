using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Nuxeo
{
    internal record DocumentGetArguments
    {
        public string? Path { get; init; }

        public string? Uid { get; init; }
    }

    internal class DocumentGetAction : NuxeoAction<DocumentGetArguments>
    {
        public DocumentGetAction(NuxeoConfig config) : base(config)
        {
        }

        protected override async Task Act(IFlowContext context, DocumentGetArguments? arguments, Client client)
        {
            if (arguments?.Path is null && arguments?.Uid is null)
                throw new ArgumentNullException(nameof(arguments));

            var document = (arguments.Path is not null
                ? client.DocumentFromPath(arguments.Path)
                : client.DocumentFromUid(arguments.Uid))
                .AddContentEnricher("children")
                .AddContentEnricher("documentURL")
                .AddSchema("*");

            var documentEntity = await document.Get();

            await context.SetData(documentEntity);
        }
    }
}