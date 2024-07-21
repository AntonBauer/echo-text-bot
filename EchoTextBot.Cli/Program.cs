using Telegram.BotAPI.GettingUpdates;
using Whisper.net;
using TgBot;
using DotNetEnv;
using EchoTextBot.Cli;

const string EnvFilePath = "../assets/";
const string envVariablesPrefix = "ECHO_TEXT_BOT";
var cancellationSource = new CancellationTokenSource();
var cancellationToken = cancellationSource.Token;

LoadEnv();
var settings = EchoSettings.Create();
var bot = TelegramBot.Create(envVariablesPrefix,
                             IsUpdateRelevant,
                             UpdateProcessor);
using var whisperFactory = WhisperFactory.FromPath(settings.WhisperModelPath);

static void LoadEnv()
{
    if (!File.Exists(EnvFilePath))
        return;

    Env.Load(EnvFilePath);
}

static bool IsUpdateRelevant(Update update) =>
    update.Message?.ExternalReply?.Audio is not null
    && !string.IsNullOrEmpty(update.Message.Text);

static async Task UpdateProcessor(Update update, CancellationToken cancellationToken)
{
    // var data = await tgClient.ExtractData(update, HttpClient, settings.TgToken, cancellationToken);
    // var converted = await data.Convert();
    // var transcription = await converted.Transcribe(whisperFactory, cancellationToken);
    // await update.SendResult(tgClient, transcription, cancellationToken);
}

await bot.Run(cancellationToken);
