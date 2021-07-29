using Humanizer;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Imap
{
    internal record SearchArguments
    {
        public string Folder { get; init; } = "INBOX";

        public string? Query { get; init; }
    }

    internal class SearchAction : ImapAction<SearchArguments>
    {
        public override string Name { get; } = "search";

        protected override async Task Act(IFlowContext context, ImapClient client, SearchArguments arguments)
        {
            if (arguments.Query is null)
                throw new ArgumentNullException(nameof(arguments.Query));

            var searchQuery = (SearchQuery?)typeof(SearchQuery).GetField(arguments.Query)?.GetValue(null);

            if (searchQuery is null)
                throw new ArgumentException();

            var folder = await client.GetFolderAsync(arguments.Folder);
            await folder.OpenAsync(FolderAccess.ReadOnly);
            var ids = await folder.SearchAsync(searchQuery);
            var messageSummaries = await folder.FetchAsync(ids, MessageSummaryItems.Full | MessageSummaryItems.UniqueId);
            var messages = await messageSummaries.Select(o => MailDto.FromSummary(o, folder, default)).ToList();

            await context.SetData(messages);
        }
    }
}