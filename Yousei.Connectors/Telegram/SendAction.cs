using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal class SendAction : FlowAction<TelegramConnection, SendArguments>
    {
        public override string Name { get; } = "sendtextmessage";

        protected override async Task Act(IFlowContext context, TelegramConnection connection, SendArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var chatId = await arguments.ChatId.Resolve<ChatId>(context);
            var text = await arguments.Text.Resolve<string>(context);

            var message = await connection.TelegramBotClient.SendTextMessageAsync(chatId, text);
            await context.SetData(message);
        }
    }
}