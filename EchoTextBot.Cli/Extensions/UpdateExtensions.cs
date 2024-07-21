using System.Text;
using EchoTextBot.Cli.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace EchoTextBot.Cli.Extensions;

internal static class UpdateExtensions
{
    public static async Task<TranscribeData> ExtractData(this TelegramBotClient tgClient,
                                                         Update update,
                                                         HttpClient httpClient,
                                                         string botToken,
                                                         CancellationToken cancellationToken)
    {
        var language = update.ExtractLanguage();

        var filePath = await update.PrepareAudioForDownload(tgClient, cancellationToken);
        var dataStream = await httpClient.DownloadAudio(filePath, botToken, cancellationToken);

        return new TranscribeData(language, dataStream);
    }

    public static async Task SendResult(this Update update,
                                        TelegramBotClient client,
                                        string transcription,
                                        CancellationToken cancellationToken)
    {
        var chatId = update.Message.Chat.Id;
        var fileBytes = Encoding.UTF8.GetBytes(transcription);

        var file = new InputFile(fileBytes, "trancription.txt");
        await client.SendDocumentAsync(chatId, file, cancellationToken: cancellationToken);
    }

    private static async Task<string> PrepareAudioForDownload(this Update update,
                                                              TelegramBotClient client,
                                                              CancellationToken cancellationToken)
    {
        var file = await client.GetFileAsync(update.Message.ExternalReply.Audio.FileId, cancellationToken);
        return file.FilePath ?? string.Empty;
    }

    private static string ExtractLanguage(this Update update) =>
        string.IsNullOrEmpty(update.Message.Text)
            ? "en"
            : update.Message.Text;

    private static async Task<Stream> DownloadAudio(this HttpClient client,
                                                    string filePath,
                                                    string botToken,
                                                    CancellationToken cancellationToken)
    {
        var url = $"https://api.telegram.org/file/bot{botToken}/{filePath}";
        return await client.GetStreamAsync(url, cancellationToken);
    }
}
