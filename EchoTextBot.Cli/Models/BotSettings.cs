using DotNetEnv;

namespace EchoTextBot.Cli.Models;

internal readonly record struct BotSettings
{
    private const string EnvFilePath = "../assets/.env";
    private const string TgTokenEnvVariable = "ECHO_TEXT_BOT_TG_TOKEN";
    private const string FetchIntervalEnvVariable = "ECHO_TEXT_BOT_FETCH_INTERVAL";
    private const string WhisperModelPathEnvVariable = "ECHO_TEXT_BOT_WHISPER_MODEL_PATH";
    private const string DbFilePathEvnVariable = "ECHO_TEXT_BOT_DB_FILE_PATH";

    public string TgToken { get; }
    public int FetchIntervalMs { get; }
    public string WhisperModelPath { get; }
    public string DbConnectionString { get; }

    private BotSettings(string tgToken,
                        string dbConnectionString,
                        int fetchIntervalMs,
                        string whisperModelPath)
    {
        TgToken = tgToken;
        DbConnectionString = dbConnectionString;
        FetchIntervalMs = fetchIntervalMs;
        WhisperModelPath = whisperModelPath;
    }

    public static BotSettings Create()
    {
        LoadEnv();

        var tgToken = Environment.GetEnvironmentVariable(TgTokenEnvVariable) ?? string.Empty;

        var fetchIntervalRaw = Environment.GetEnvironmentVariable(FetchIntervalEnvVariable);
        var fetchInterval = string.IsNullOrEmpty(fetchIntervalRaw)
            ? 5000
            : int.Parse(fetchIntervalRaw);

        var whisperModelPath = Environment.GetEnvironmentVariable(WhisperModelPathEnvVariable) ?? string.Empty;

        var dbFilePath = Environment.GetEnvironmentVariable(DbFilePathEvnVariable) ?? string.Empty;
        var dbConnectionString = $"Data Source={dbFilePath}";

        return new(tgToken, dbConnectionString, fetchInterval, whisperModelPath);
    }

    private static void LoadEnv()
    {
        if (!File.Exists(EnvFilePath))
            return;

        Env.Load(EnvFilePath);
    }
}