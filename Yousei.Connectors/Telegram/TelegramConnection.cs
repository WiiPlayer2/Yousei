using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal class TelegramConnection : IConnection, IUpdateHandler
    {
        private readonly Subject<Exception> errors = new();

        private readonly Subject<Update> updates = new();

        public TelegramConnection(TelegramBotClient telegramBotClient)
        {
            var connectObservable = CreateConnectionObservable(telegramBotClient);
            OnUpdate = WrapEvent(connectObservable, updates);
            OnError = WrapEvent(connectObservable, errors);
            TelegramBotClient = telegramBotClient;
        }

        public UpdateType[]? AllowedUpdates { get; } = new UpdateType[0];

        public IObservable<Exception> OnError { get; }

        public IObservable<Update> OnUpdate { get; }

        public TelegramBotClient TelegramBotClient { get; }

        public void Dispose()
        {
            updates.Dispose();
            errors.Dispose();
        }

        public Task HandleError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            errors.OnNext(exception);
            return Task.CompletedTask;
        }

        public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            updates.OnNext(update);
            return Task.CompletedTask;
        }

        private IObservable<Unit> CreateConnectionObservable(TelegramBotClient telegramBotClient)
            => Observable.Create<Unit>(async (_, cancellationToken) =>
                {
                    await telegramBotClient.ReceiveAsync(this, cancellationToken: cancellationToken);
                })
                .Publish()
                .RefCount(TimeSpan.FromSeconds(1));

        private IObservable<T> WrapEvent<T>(IObservable<Unit> connectionObservable, ISubject<T> subject)
            where T : class
            => Observable.DeferAsync(cancellationToken =>
                {
                    var subscription = connectionObservable.Subscribe(_ => { });
                    cancellationToken.Register(subscription.Dispose);
                    return Task.FromResult<IObservable<T>>(subject);
                });
    }
}