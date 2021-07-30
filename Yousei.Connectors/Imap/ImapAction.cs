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
    internal abstract class ImapAction<TArguments> : FlowAction<ObjectConnection<ImapConfiguration>, TArguments>
        where TArguments : new()
    {
        protected sealed override async Task Act(IFlowContext context, ObjectConnection<ImapConfiguration> connection, TArguments? arguments)
        {
            var config = connection.Object;
            using var client = new ImapClient();
            await client.ConnectAsync(config.Host, config.Port);
            await client.AuthenticateAsync(config.Username, config.Password);

            await Act(context, client, arguments ?? new TArguments());
        }

        protected abstract Task Act(IFlowContext context, ImapClient client, TArguments arguments);
    }
}