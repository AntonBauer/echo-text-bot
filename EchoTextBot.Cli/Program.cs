using Telegram.BotAPI.GettingUpdates;
using Whisper.net;
using TgBot;
using DotNetEnv;
using EchoTextBot.Cli;
using EchoTextBot.Cli.Extensions;

const string EnvFilePath = "./../assets/.env";
const string envVariablesPrefix = "ECHO_TEXT_BOT";
var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

LoadEnv();
var settings = EchoSettings.Create();
using var whisperFactory = WhisperFactory.FromPath(settings.WhisperModelPath);
var bot = TelegramBot.Create(envVariablesPrefix,
                             IsUpdateRelevant,
                             (update, bot, cancellationToken) =>
                                UpdateProcessor(update, bot, whisperFactory, cancellationToken)
                            );

static void LoadEnv()
{
    if (!File.Exists(EnvFilePath))
        return;

    Env.Load(EnvFilePath);
}

static bool IsUpdateRelevant(Update update) =>
    update.Message?.ExternalReply?.Audio is not null
    && !string.IsNullOrEmpty(update.Message.Text);

static async Task UpdateProcessor(Update update,
                                  TelegramBot bot,
                                  WhisperFactory whisperFactory,
                                  CancellationToken cancellationToken)
{
    var data = await update.ExtractData(bot, cancellationToken);
    var converted = await data.Convert();
    var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
    await bot.SendAsFileTo(transcription, "transcription.txt", update.Message!.Chat.Id, cancellationToken);
}

await bot.Run(cancellationToken);
