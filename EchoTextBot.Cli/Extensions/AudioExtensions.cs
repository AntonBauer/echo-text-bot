using EchoTextBot.Cli.Models;
using FFMpegCore;
using FFMpegCore.Pipes;

namespace EchoTextBot.Cli.Extensions;

internal static class AudioExtensions
{
    public static async Task<TranscribeData> Convert(this TranscribeData data)
    {
        var convertedData = new MemoryStream();

        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(data.Data))
            .OutputToPipe(new StreamPipeSink(convertedData),
                          options => options.WithAudioSamplingRate(16000)
                                            .ForceFormat("wav"))
            .ProcessAsynchronously();
        convertedData.Seek(0, SeekOrigin.Begin);

        return data with { Data = convertedData };
    }
}