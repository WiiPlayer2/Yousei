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
    internal class DeleteAction : FlowAction<DeleteArguments>
    {
        private readonly Func<CancellationToken, Task<ImapClient>> createClient;

        public DeleteAction(Func<CancellationToken, Task<ImapClient>> createClient)
        {
            this.createClient = createClient;
        }

        protected override async Task Act(IFlowContext context, DeleteArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            using var client = await createClient(default);
            var folderPath = await arguments.Folder.Resolve<string>(context);
            var id = await arguments.ID.Resolve<uint>(context);

            var uniqueId = new UniqueId(id);
            var folder = await client.GetFolderAsync(folderPath);
            await folder.OpenAsync(FolderAccess.ReadWrite);
            await folder.AddFlagsAsync(uniqueId, MessageFlags.Deleted, true);
            await folder.ExpungeAsync();
        }
    }
}