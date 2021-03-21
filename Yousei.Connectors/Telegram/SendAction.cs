using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal class SendAction : FlowAction<SendArguments>
    {
        private readonly TelegramBotClient telegramBotClient;

        public SendAction(TelegramBotClient telegramBotClient)
        {
            this.telegramBotClient = telegramBotClient;
        }

        protected override async Task Act(IFlowContext context, SendArguments arguments)
        {
            var chatId = await arguments.ChatId.Resolve<ChatId>(context);
            var text = await arguments.Text.Resolve<string>(context);

            var message = await telegramBotClient.SendTextMessageAsync(chatId, text);
            await context.SetData(message);
        }
    }
}