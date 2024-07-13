using DotNetEnv;

internal readonly record struct BotSettings
{
    private const string EnvFilePath = "../assets/.env";
    private const string TgTokenEnvVariable = "ECHO_TEXT_BOT_TG_TOKEN";
    private const string FetchIntervalEnvVariable = "ECHO_TEXT_BOT_FETCH_INTERVAL";
    private const string WhisperModelPathEnvVariable = "ECHO_TEXT_BOT_WHISPER_MODEL_PATH";

    public string TgToken { get; }
    public int? Offset { get; }
    public int FetchIntervalMs { get;}
    public string WhisperModelPath { get; }

    private BotSettings(string tgToken,
                        int? offset,
                        int fetchIntervalMs,
                        string whisperModelPath)
    {
        TgToken = tgToken;
        Offset = offset;
        FetchIntervalMs = fetchIntervalMs;
        WhisperModelPath = whisperModelPath;
    }

    public static BotSettings Create()
    {
        LoadEnv();

        var tgToken = Environment.GetEnvironmentVariable(TgTokenEnvVariable) ?? string.Empty;
        var offset = default(int?);

        var fetchIntervalRaw = Environment.GetEnvironmentVariable(FetchIntervalEnvVariable);
        var fetchInterval = string.IsNullOrEmpty(fetchIntervalRaw)
            ? 5000
            : int.Parse(fetchIntervalRaw);

        var whisperModelPath = Environment.GetEnvironmentVariable(WhisperModelPathEnvVariable) ?? string.Empty;
        
        return new(tgToken, offset, fetchInterval, whisperModelPath);
    }

    private static void LoadEnv()
    {
        if(!File.Exists(EnvFilePath))
            return;

        Env.Load(EnvFilePath);
    }
}