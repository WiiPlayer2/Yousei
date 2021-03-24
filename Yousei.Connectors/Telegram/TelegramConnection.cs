using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Yousei.Core;

namespace Yousei.Connectors.Telegram
{
    internal class TelegramConnection : SimpleConnection
    {
        public TelegramConnection(TelegramBotClient telegramBotClient)
        {
            var connectObservable = CreateConnectionObservable(telegramBotClient);
            OnMessage = WrapEvent<MessageEventArgs>(connectObservable, telegramBotClient, nameof(TelegramBotClient.OnMessage));
            OnUpdate = WrapEvent<UpdateEventArgs>(connectObservable, telegramBotClient, nameof(TelegramBotClient.OnUpdate));

            AddTrigger("onmessage", new ObservableTrigger(OnMessage.Select(o => o.Message)));
            AddTrigger("onupdate", new ObservableTrigger(OnUpdate.Select(o => o.Update)));
            AddAction("sendtextmessage", new SendAction(telegramBotClient));
        }

        public IObservable<MessageEventArgs> OnMessage { get; }

        public IObservable<UpdateEventArgs> OnUpdate { get; }

        private IObservable<Unit> CreateConnectionObservable(TelegramBotClient telegramBotClient)
            => Observable.Create<Unit>(async (observer, cancellationToken) =>
                {
                    telegramBotClient.StartReceiving(cancellationToken: cancellationToken);
                    await Task.Delay(-1, cancellationToken);
                })
                .Publish()
                .RefCount(TimeSpan.FromSeconds(1));

        private IObservable<T> WrapEvent<T>(IObservable<Unit> connectionObservable, object target, string eventName)
                    where T : class
        {
            var eventObservable = Observable.FromEventPattern<T>(target, eventName).Select(o => o.EventArgs);
            return Observable.Merge(connectionObservable.Select(_ => default(T)), eventObservable);
        }
    }
}