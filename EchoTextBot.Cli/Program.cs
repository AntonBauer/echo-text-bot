using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

var botToken = "";
var offset = default(int?);

var tgClient = new TelegramBotClient(botToken);
var httpClient = new HttpClient();

while (true)
{
    var allUpdates = (await tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();

    if (allUpdates.Length > 0)
    {
        foreach(var update in allUpdates.Where(UpdateExtensions.IsRelevant))
        {
            var language = update.ExtractLanguage();
            var filePath = await update.PrepareAudioForDownload(tgClient, cancellationToken);
            var file = await httpClient.DownloadFile(filePath, botToken, cancellationToken);
        }

        offset = allUpdates[^1].UpdateId + 1;
        continue;
    }

    await Task.Delay(5000);
}

// convert to format used by whisper

// transcribe by whisper
// output to file

// Answer to original message
