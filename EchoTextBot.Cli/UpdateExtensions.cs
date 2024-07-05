using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace EchoTextBot.Cli;

internal static class UpdateExtensions
{
    public static bool IsRelevant(this Update update) =>
        update.Message?.ExternalReply?.Audio is not null
        && !string.IsNullOrEmpty(update.Message.Text);

    public static async Task<string> PrepareAudioForDownload(this Update update,
                                          TelegramBotClient client,
                                          CancellationToken cancellationToken)
    {
        var file = await client.GetFileAsync(update.Message.ExternalReply.Audio.FileId, cancellationToken);
        return file.FilePath ?? string.Empty;
    }

    public static string ExtractLanguage(this Update update) => update.Message.Text;

    public static async Task<Stream> DownloadFile(this HttpClient client,
                                                  string filePath,
                                                  string botToken,
                                                  CancellationToken cancellationToken)
    {
        var url = $"https://api.telegram.org/file/{botToken}/{filePath}";
        var response = await client.GetAsync(url, cancellationToken);
        var dataStream = new MemoryStream();
        await response.Content.CopyToAsync(dataStream, cancellationToken);

        return dataStream;
    }
}