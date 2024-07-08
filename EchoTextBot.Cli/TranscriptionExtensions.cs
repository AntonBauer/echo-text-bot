using System.Text;
using Whisper.net;

namespace EchoTextBot.Cli;

internal static class TranscriptionExtensions
{
    public static async Task<string> Transcribe(this TranscribeData data,
                                                WhisperFactory whisperFactory,
                                                CancellationToken cancellationToken)
    {
        var stringBuilder = new StringBuilder();

        await using var transcriber = whisperFactory.CreateBuilder()
                                                    .WithLanguage(data.Language)
                                                    .Build();

        await foreach(var output in transcriber.ProcessAsync(data.Data, cancellationToken))
            stringBuilder.AppendLine(output.Text);

        return stringBuilder.ToString();
    }
}