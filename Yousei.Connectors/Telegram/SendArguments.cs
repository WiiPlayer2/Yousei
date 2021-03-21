using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal record SendArguments
    {
        public IParameter ChatId { get; init; }

        public IParameter Text { get; init; }
    }
}