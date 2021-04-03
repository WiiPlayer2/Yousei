namespace Yousei.Connectors.Telegram
{
    public record Config
    {
        public string Token { get; init; } = string.Empty;
    }
}