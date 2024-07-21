using Telegram.BotAPI.GettingUpdates;
using Whisper.net;
using TgBot;
using EchoTextBot.Cli.Extensions;

var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

// Tools


var bot = TelegramBot.Create(UpdateExtensions.IsRelevant, UpdateProcessor);
using var whisperFactory = WhisperFactory.FromPath(settings.WhisperModelPath);

private static Task UpdateProcessor(Update update)
{

    var data = await tgClient.ExtractData(update, HttpClient, settings.TgToken, cancellationToken);
    var converted = await data.Convert();
    var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
    await update.SendResult(tgClient, transcription, cancellationToken);
}

await bot.Run(cancellationToken);
