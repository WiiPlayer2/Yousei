using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Telegram
{
    internal record SendArguments
    {
        public IParameter ChatId { get; init; } = DefaultParameter.Instance;

        public IParameter Text { get; init; } = DefaultParameter.Instance;
    }
}