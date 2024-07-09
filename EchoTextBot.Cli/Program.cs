using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Whisper.net;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

// Environment Data
var settings = BotSettings.Create();
var offset = settings.Offset;

// Tools
var tgClient = new TelegramBotClient(settings.TgToken);
using var httpClient = new HttpClient();
using var whisperFactory = WhisperFactory.FromPath(settings.WhisperModelPath);

while (true)
{
    var allUpdates = (await tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();

    if (allUpdates.Length > 0)
    {
        foreach(var update in allUpdates.Where(UpdateExtensions.IsRelevant))
        {
            var data = await tgClient.ExtractData(update, httpClient, settings.TgToken, cancellationToken);
            var converted = await data.Convert();
            var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
            await update.SendResult(tgClient, transcription, cancellationToken);
        }

        offset = allUpdates[^1].UpdateId + 1;
        continue;
    }

    await Task.Delay(settings.FetchIntervalMs);
}
