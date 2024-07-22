using EchoTextBot.Cli.Models;
using Telegram.BotAPI.GettingUpdates;
using TgBot;

namespace EchoTextBot.Cli.Extensions;

internal static class UpdateExtensions
{
    public static async Task<TranscribeData> ExtractData(this Update update,
                                                         TelegramBot bot,
                                                         CancellationToken cancellationToken)
    {
        var language = update.ExtractLanguage();
        var dataStream = await bot.DownloadAudioFromExternalReply(update,
                                                                  cancellationToken);

        return new TranscribeData(language, dataStream);
    }

    private static string ExtractLanguage(this Update update) =>
        string.IsNullOrEmpty(update.Message!.Text)
            ? "en"
            : update.Message.Text;
}
