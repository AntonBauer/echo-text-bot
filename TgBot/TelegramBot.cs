namespace TgBot;

public sealed class TelegramBot
{
    private readonly TelegramBotClient _tgClient;
    private readonly BotSettings _settings;
    private readonly HttpClient _httpClient;

    public async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {

        }
    }
}
