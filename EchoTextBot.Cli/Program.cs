using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Whisper.net;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

// Environment Data
var botToken = "";
var offset = default(int?);
var fetchInterval = 5000;
var whisperModelPath = "";

// Tools
var tgClient = new TelegramBotClient(botToken);
using var httpClient = new HttpClient();
using var whisperFactory = WhisperFactory.FromPath(whisperModelPath);

while (true)
{
    var allUpdates = (await tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();

    if (allUpdates.Length > 0)
    {
        foreach(var update in allUpdates.Where(UpdateExtensions.IsRelevant))
        {
            var data = await tgClient.ExtractData(update, httpClient, botToken, cancellationToken);
            var converted = await data.Convert();
            var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
            await update.SendResult(tgClient, transcription, cancellationToken);
        }

        offset = allUpdates[^1].UpdateId + 1;
        continue;
    }

    await Task.Delay(fetchInterval);
}
