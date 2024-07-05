using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace EchoTextBot.Cli;

internal static class UpdateExtensions
{
    public static async Task ExtractAudio(this Update update,
                                          TelegramBotClient client,
                                          CancellationToken cancellationToken)
    {
        var file = await client.GetFileAsync(update.Message.Voice.FileId, cancellationToken);
        // Download file from https://api.telegram.org/file/bot<token>/<file_path> 
    }

    public static string ExtractLanguage(this Update update) =>
        update.Message.Text ?? string.Empty;
}