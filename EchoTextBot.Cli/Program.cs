using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Whisper.net;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

// Environment Data
var settings = BotSettings.Create();

// Tools
var tgClient = new TelegramBotClient(settings.TgToken);
using var httpClient = new HttpClient();
using var whisperFactory = WhisperFactory.FromPath(settings.WhisperModelPath);

var offset = default(int?);
while (true)
{
    try
    {
        var allUpdates = (await tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();

        if (allUpdates.Length > 0)
        {
            foreach (var update in allUpdates.Where(UpdateExtensions.IsRelevant))
            {
                var data = await tgClient.ExtractData(update, httpClient, settings.TgToken, cancellationToken);
                var converted = await data.Convert();
                var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
                await update.SendResult(tgClient, transcription, cancellationToken);
            }

            offset = allUpdates[^1].UpdateId + 1;
            continue;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    await Task.Delay(settings.FetchIntervalMs);
}
