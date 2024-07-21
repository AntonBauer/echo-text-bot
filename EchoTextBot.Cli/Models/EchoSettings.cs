namespace EchoTextBot.Cli;

internal readonly record struct EchoSettings
{
    private const string WhisperModelPathEnvVariable = "ECHO_TEXT_BOT_WHISPER_MODEL_PATH";

    public string WhisperModelPath { get; }

    private EchoSettings(string whisperModelPath)
    {
        WhisperModelPath = whisperModelPath;
    }

    public static EchoSettings Create()
    {
        var whisperModelPath = Environment.GetEnvironmentVariable(WhisperModelPathEnvVariable) ?? string.Empty;

        return new(whisperModelPath);
    }
}

