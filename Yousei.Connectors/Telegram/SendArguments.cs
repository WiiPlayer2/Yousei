using Telegram.Bot.Types;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal record SendArguments
    {
        public IParameter<ChatId> ChatId { get; init; } = DefaultParameter<ChatId>.Instance;

        public IParameter<string> Text { get; init; } = DefaultParameter<string>.Instance;
    }
}