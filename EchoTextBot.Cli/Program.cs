using EchoTextBot.Cli;
using Telegram.BotAPI;

var cancellationSource = new CancellationTokenSource();
var token = cancellationSource.Token;

var botToken = "";
var offset = default(int?);

var client = new TelegramBotClient(botToken);

while (true)
{
    var updates = (await client.GetRelevantUpdates(offset, token)).ToArray();
    offset = updates[^1].UpdateId + 1;
    await Task.Delay(5000);
}

// get messages
// fiilter everything but audio

// convert to format used by whisper

// transcribe by whisper
// output to file

// Answer to original message
