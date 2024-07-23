using System.Text;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using TgBot.Services;

namespace TgBot;

public sealed class TelegramBot
{
    private const string TelegramFilesUrl = "https://api.telegram.org/file/bot";

    private readonly TelegramBotClient _tgClient;
    private readonly BotSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly OffsetService _offsetService;

    private readonly Func<Update, bool> _isUpdateRelevant;
    private readonly Func<Update, TelegramBot, CancellationToken, Task> _updateProcessor;

    private TelegramBot(TelegramBotClient tgClient,
                        BotSettings settings,
                        HttpClient httpClient,
                        OffsetService offsetService,
                        Func<Update, bool> isUpdateRelevant,
                        Func<Update, TelegramBot, CancellationToken, Task> updateProcessor)
    {
        _tgClient = tgClient;
        _settings = settings;
        _httpClient = httpClient;
        _offsetService = offsetService;
        _isUpdateRelevant = isUpdateRelevant;
        _updateProcessor = updateProcessor;
    }

    public static TelegramBot Create(string envVariablesPrefix,
                                     Func<Update, bool> isUpdateRelevant,
                                     Func<Update, TelegramBot, CancellationToken, Task> updateProcessor)
    {
        var settings = BotSettings.Create(envVariablesPrefix);
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
                    await _updateProcessor(update, this, cancellationToken);

                offset = allUpdates[^1].UpdateId + 1;
                await _offsetService.SaveOffset(offset.Value, cancellationToken);
                continue;
            }

            await Task.Delay(_settings.FetchIntervalMs, cancellationToken);
        }
    }

    public async Task<Stream> DownloadAudioFromExternalReply(Update update,
                                                             CancellationToken cancellationToken)
    {
        var filePath = await _tgClient.GetFileAsync(update.Message.ExternalReply.Audio.FileId,
                                                    cancellationToken);
        var url = $"{TelegramFilesUrl}{_settings.TgToken}/{filePath}";
        return await _httpClient.GetStreamAsync(url, cancellationToken);
    }

    public async Task SendAsFileTo(string text,
                                   string fileName,
                                   long chatId,
                                   CancellationToken cancellationToken)
    {
        var fileBytes = Encoding.UTF8.GetBytes(text);
        var file = new InputFile(fileBytes, fileName);

        await _tgClient.SendDocumentAsync(chatId,
                                          file,
                                          cancellationToken: cancellationToken);
    }

    private async Task<Update[]> LoadUpdates(int? offset,
                                             CancellationToken cancellationToken) =>
        (await _tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();
}
