using Telegram.Bot;
using Yousei.Core;

namespace Yousei.Connectors.Telegram
{
    internal class TelegramConnection : SimpleConnection
    {
        private readonly TelegramBotClient telegramBotClient;

        public TelegramConnection(TelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }
    }
}