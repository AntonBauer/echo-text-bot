using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using TgBot.Services;

namespace TgBot;

public sealed class TelegramBot
{
    private readonly TelegramBotClient _tgClient;
    private readonly BotSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly OffsetService _offsetService;

    private readonly Func<Update, bool> _isUpdateRelevant;
    private readonly Func<Update, Task> _updateProcessor;

    private TelegramBot(TelegramBotClient tgClient,
                        BotSettings settings,
                        HttpClient httpClient,
                        OffsetService offsetService,
                        Func<Update, bool> isUpdateRelevant,
                        Func<Update, Task> updateProcessor)
    {
        _tgClient = tgClient;
        _settings = settings;
        _httpClient = httpClient;
        _offsetService = offsetService;
        _isUpdateRelevant = isUpdateRelevant;
        _updateProcessor = updateProcessor;
    }

    public static TelegramBot Create(Func<Update, bool> isUpdateRelevant,
                                     Func<Update, Task> updateProcessor)
    {
        var settings = BotSettings.Create();
        var tgClient = new TelegramBotClient(settings.TgToken);
        var httpClient = new HttpClient();
        var offsetService = new OffsetService(settings.DbConnectionString);

        return new(tgClient,
                   settings,
                   httpClient,
                   offsetService,
                   isUpdateRelevant,
                   updateProcessor);
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        var offset = await _offsetService.ReadOffset(cancellationToken);

        while (!cancellationToken.IsCancellationRequested)
        {
            var allUpdates = await LoadUpdates(offset, cancellationToken);
            if (allUpdates.Length > 0)
            {
                foreach (var update in allUpdates.Where(_isUpdateRelevant))
                {
                    await _updateProcessor(update);
                }

                offset = allUpdates[^1].UpdateId + 1;
                await _offsetService.SaveOffset(offset.Value, cancellationToken);
                continue;
            }

            await Task.Delay(_settings.FetchIntervalMs, cancellationToken);
        }
    }

    private async Task<Update[]> LoadUpdates(int? offset,
                                             CancellationToken cancellationToken) =>
        (await _tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();
}
