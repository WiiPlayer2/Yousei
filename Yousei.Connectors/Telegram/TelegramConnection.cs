using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal class TelegramConnection : IConnection
    {
        public TelegramConnection(TelegramBotClient telegramBotClient)
        {
            var connectObservable = CreateConnectionObservable(telegramBotClient);
            OnMessage = WrapEvent<MessageEventArgs>(connectObservable, telegramBotClient, nameof(TelegramBotClient.OnMessage));
            OnUpdate = WrapEvent<UpdateEventArgs>(connectObservable, telegramBotClient, nameof(TelegramBotClient.OnUpdate));
            TelegramBotClient = telegramBotClient;
        }

        public IObservable<MessageEventArgs> OnMessage { get; }

        public IObservable<UpdateEventArgs> OnUpdate { get; }

        public TelegramBotClient TelegramBotClient { get; }

        public void Dispose()
        {
        }

        private IObservable<Unit> CreateConnectionObservable(TelegramBotClient telegramBotClient)
            => Observable.Create<Unit>(async (_, cancellationToken) =>
                {
                    telegramBotClient.StartReceiving(cancellationToken: cancellationToken);
                    await Task.Delay(-1, cancellationToken);
                })
                .Publish()
                .RefCount(TimeSpan.FromSeconds(1));

        private IObservable<T> WrapEvent<T>(IObservable<Unit> connectionObservable, object target, string eventName)
            where T : class
            => Observable.DeferAsync(cancellationToken =>
                {
                    var subscription = connectionObservable.Subscribe(_ => { });
                    cancellationToken.Register(subscription.Dispose);
                    var eventObservable = Observable.FromEventPattern<T>(target, eventName).Select(o => o.EventArgs);
                    return Task.FromResult(eventObservable);
                });
    }
}