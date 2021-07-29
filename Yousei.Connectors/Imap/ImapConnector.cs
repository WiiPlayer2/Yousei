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
    public class ImapConnector : SimpleConnector<ObjectConnection<ImapConfiguration>, ImapConfiguration>
    {
        public ImapConnector()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AddTrigger(new ObservableTrigger<ObjectConnection<ImapConfiguration>>("subscribe", c => CreateSubscribeObservable(c.Object)));
            AddAction<DeleteAction>();
            AddAction<FetchAction>();
            AddAction<SearchAction>();
            AddAction<FlagAction>();
        }

        public override string Name { get; } = "imap";

        protected override ObjectConnection<ImapConfiguration>? CreateConnection(ImapConfiguration configuration)
            => ObjectConnection.From(configuration);

        private async Task<ImapClient> Connect(ImapConfiguration config, CancellationToken cancellationToken)
        {
            var client = new ImapClient();
            await client.ConnectAsync(config.Host, config.Port, cancellationToken: cancellationToken);
            await client.AuthenticateAsync(config.Username, config.Password, cancellationToken: cancellationToken);
            return client;
        }

        private IObservable<object> CreateSubscribeObservable(ImapConfiguration config)
            => Observable.Create<object>(async (observer, cancellationToken) =>
            {
                using var client = await Connect(config, cancellationToken);

                await client.Inbox.OpenAsync(FolderAccess.ReadWrite, cancellationToken);

                var messageCount = client.Inbox.Count;

                var done = new CancellationTokenSource(9.Minutes());
                client.Inbox.CountChanged += (_, __) =>
                {
                    if (messageCount < client.Inbox.Count)
                        done.Cancel();
                    else
                        messageCount = client.Inbox.Count;
                };
                await client.Inbox.SubscribeAsync(cancellationToken);

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (client.Capabilities.HasFlag(ImapCapabilities.Idle))
                    {
                        await client.IdleAsync(done.Token, cancellationToken);
                    }
                    else
                    {
                        using var doneOrCancelled = CancellationTokenSource.CreateLinkedTokenSource(done.Token, cancellationToken);
                        await Task.Delay(1.Minutes(), doneOrCancelled.Token).IgnoreCancellation();
                        await client.NoOpAsync(cancellationToken);
                    }

                    var newMessages = await client.Inbox.FetchAsync(messageCount, -1, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, cancellationToken);
                    foreach (var message in newMessages)
                    {
                        var dto = await MailDto.FromSummary(message, client.Inbox, cancellationToken);
                        observer.OnNext(dto);
                    }
                    messageCount += newMessages.Count;

                    done.Dispose();
                    done = new CancellationTokenSource(9.Minutes());
                }
            })
                .Publish()
                .RefCount();
    }
}