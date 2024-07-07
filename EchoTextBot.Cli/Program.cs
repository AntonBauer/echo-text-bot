using EchoTextBot.Cli;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

var botToken = "6836029113:AAHwys0NxhqkPWRI41puPtHkS2FBdWXsHss";
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
            var data = await tgClient.ExtractData(update, httpClient, botToken, cancellationToken);
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
