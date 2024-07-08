using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

// Environment Data
var botToken = "";
var offset = default(int?);
var fetchInterval = 5000;

var tgClient = new TelegramBotClient(botToken);
var httpClient = new HttpClient();

while (true)
{
    var allUpdates = (await tgClient.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).ToArray();

    if (allUpdates.Length > 0)
    {
        foreach(var update in allUpdates.Where(UpdateExtensions.IsRelevant))
        {
            var data = await tgClient.ExtractData(update, httpClient, botToken, cancellationToken);
            var converted = await data.Convert();
        }

        offset = allUpdates[^1].UpdateId + 1;
        continue;
    }

    await Task.Delay(fetchInterval);
}

// transcribe by whisper
// output to file

// Answer to original message
