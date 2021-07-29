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
    internal record FlagArguments
    {
        public string Folder { get; init; } = "INBOX";

        public List<uint> IDs { get; init; } = new();

        public List<MessageFlags> Add { get; init; } = new();

        public List<MessageFlags> Remove { get; init; } = new();
    }

    internal class FlagAction : ImapAction<FlagArguments>
    {
        public override string Name { get; } = "flag";

        protected override async Task Act(IFlowContext context, ImapClient client, FlagArguments arguments)
        {
            var uniqueIds = arguments.IDs.Select(o => new UniqueId(o)).ToList();
            var addFlags = arguments.Add.Aggregate(default(MessageFlags), (acc, curr) => acc | curr);
            var removeFlags = arguments.Remove.Aggregate(default(MessageFlags), (acc, curr) => acc | curr);

            var folder = await client.GetFolderAsync(arguments.Folder);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            await folder.AddFlagsAsync(uniqueIds, addFlags, true);
            await folder.RemoveFlagsAsync(uniqueIds, removeFlags, true);
        }
    }
}