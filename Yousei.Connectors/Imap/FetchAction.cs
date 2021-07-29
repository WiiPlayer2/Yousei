using Humanizer;
using MailKit;
using MailKit.Net.Imap;
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
    internal record FetchArguments
    {
        public string Folder { get; init; } = "INBOX";

        public List<uint>? IDs { get; init; }
    }

    internal class FetchAction : ImapAction<FetchArguments>
    {
        public override string Name { get; } = "fetch";

        protected override async Task Act(IFlowContext context, ImapClient client, FetchArguments arguments)
        {
            var folder = await client.GetFolderAsync(arguments.Folder);
            await folder.OpenAsync(FolderAccess.ReadOnly);
            var messageSummaryItems = MessageSummaryItems.Full | MessageSummaryItems.UniqueId;
            var messageSummaries = arguments.IDs is not null
                ? await folder.FetchAsync(arguments.IDs.Select(o => new UniqueId(o)).ToList(), messageSummaryItems)
                : await folder.FetchAsync(0, -1, messageSummaryItems);
            var messages = await messageSummaries.Select(o => MailDto.FromSummary(o, folder, default)).ToList();
            await context.SetData(messages);
        }
    }
}