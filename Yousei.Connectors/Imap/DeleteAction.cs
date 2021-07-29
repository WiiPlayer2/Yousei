using MailKit;
using MailKit.Net.Imap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Connectors.Imap
{
    internal record DeleteArguments
    {
        public string Folder { get; init; } = "INBOX";

        public List<uint> IDs { get; init; } = new();
    }

    internal class DeleteAction : ImapAction<DeleteArguments>
    {
        public override string Name { get; } = "delete";

        protected override async Task Act(IFlowContext context, ImapClient client, DeleteArguments arguments)
        {
            var ids = arguments.IDs.Select(o => new UniqueId(o)).ToList();
            var folder = await client.GetFolderAsync(arguments.Folder);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            await folder.AddFlagsAsync(ids, MessageFlags.Deleted, true);
            await folder.ExpungeAsync();
        }
    }
}