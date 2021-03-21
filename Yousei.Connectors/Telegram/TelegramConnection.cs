using Telegram.Bot;
using Yousei.Core;

namespace Yousei.Connectors.Telegram
{
    internal class TelegramConnection : SimpleConnection
    {
        public TelegramConnection(TelegramBotClient telegramBotClient)
        {
            AddAction("send", new SendAction(telegramBotClient));
        }
    }
}