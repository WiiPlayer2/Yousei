﻿using Humanizer;
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

    internal class ImapConnection : SimpleConnection
    {
        public ImapConnection(ImapConfiguration config)
        {
            AddTrigger("subscribe", new ObservableTrigger(CreateSubscribeObservable(config)));
        }

        private IObservable<object> CreateSubscribeObservable(ImapConfiguration config)
            => Observable.Create<object>(async (observer, cancellationToken) =>
                {
                    using var client = new ImapClient();
                    await client.ConnectAsync(config.Host, config.Port, cancellationToken: cancellationToken);
                    await client.AuthenticateAsync(config.Username, config.Password, cancellationToken: cancellationToken);

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