using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    public class TelegramConnector : SimpleConnector<Config>
    {
        public TelegramConnector() : base("telegram")
        {
        }

        protected override IConnection CreateConnection(Config configuration)
        {
            var botClient = new TelegramBotClient(configuration.Token);
            return new TelegramConnection(botClient);
        }
    }
}