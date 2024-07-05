using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace EchoTextBot.Cli;

internal static class TelegramClientExtensions
{
    public static async Task<IEnumerable<Update>> GetRelevantUpdates(this TelegramBotClient client,
                                                                      int? offset,
                                                                      CancellationToken cancellationToken) =>
        (await client.GetUpdatesAsync(offset, cancellationToken: cancellationToken)).Where(IsRelevant);

    private static bool IsRelevant(this Update update) =>
        update.Message?.Voice is not null
        && !string.IsNullOrEmpty(update.Message.Text);
}