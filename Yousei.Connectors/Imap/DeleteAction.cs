using MailKit;
using MailKit.Net.Imap;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Connectors.Imap
{
    internal class DeleteAction : ImapAction<DeleteArguments>
    {
        public override string Name { get; } = "delete";

        protected override async Task Act(IFlowContext context, ImapClient client, DeleteArguments arguments)
        {
            var folderPath = await arguments.Folder.Resolve(context);
            var id = await arguments.ID.Resolve(context);

            var uniqueId = new UniqueId(id);
            var folder = await client.GetFolderAsync(folderPath);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            await folder.AddFlagsAsync(uniqueId, MessageFlags.Deleted, true);
            await folder.ExpungeAsync();
        }
    }
}