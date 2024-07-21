namespace TgBot;

internal readonly record struct BotSettings
{
    private const string TgTokenEnvVariable = "TG_TOKEN";
    private const string FetchIntervalEnvVariable = "FETCH_INTERVAL";
    private const string DbFilePathEvnVariable = "OFFSET_DB_FILE_PATH";

    public string TgToken { get; }
    public int FetchIntervalMs { get; }
    public string DbConnectionString { get; }

    private BotSettings(string tgToken,
                        string dbConnectionString,
                        int fetchIntervalMs)
    {
        TgToken = tgToken;
        DbConnectionString = dbConnectionString;
        FetchIntervalMs = fetchIntervalMs;
    }

    public static BotSettings Create(string envVariablesNamePrefix) =>
        new(LoadBotToken(envVariablesNamePrefix),
            LoadDbConnectionString(envVariablesNamePrefix),
            LoadFetchInterval(envVariablesNamePrefix));

    private static string LoadBotToken(string envVariablesNamePrefix) =>
        Environment.GetEnvironmentVariable($"{envVariablesNamePrefix}_{TgTokenEnvVariable}")
            ?? string.Empty;

    private static int LoadFetchInterval(string envVariablesNamePrefix)
    {
        var fetchIntervalRaw = Environment.GetEnvironmentVariable($"{envVariablesNamePrefix}_{FetchIntervalEnvVariable}");
        return string.IsNullOrEmpty(fetchIntervalRaw)
            ? 5000
            : int.Parse(fetchIntervalRaw);
    }

    private static string LoadDbConnectionString(string envVariablesNamePrefix)
    {
        var dbFilePath = Environment.GetEnvironmentVariable($"{envVariablesNamePrefix}_{DbFilePathEvnVariable}")
            ?? string.Empty;
        return $"Data Source={dbFilePath}";
    }
}
