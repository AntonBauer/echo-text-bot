// See https://aka.ms/new-console-template for more information
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

// get messages
var botToken = "";
var client = new TelegramBotClient(botToken);

var updates = await client.GetUpdatesAsync();
while(true)
{
    if (updates.Any())
    {
        foreach (var update in updates)
        {
            if(update.Message is null) continue;
            await client.SendMessageAsync(update.Message.Chat.Id, update.Message?.Text ?? "Strange update");
        }

        var offset = updates.Last().UpdateId + 1;
        updates = await client.GetUpdatesAsync(offset);
    }
    else
    {
        updates = await client.GetUpdatesAsync();
    }
}
// fiilter everything but audio

// convert to format used by whisper

// transcribe by whisper
// output to file

// Answer to original message
