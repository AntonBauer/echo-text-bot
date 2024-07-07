using FFMpegCore;
using FFMpegCore.Pipes;

namespace EchoTextBot.Cli;

internal static class AudioExtensions
{
    public static async Task<TranscribeData> Convert(this TranscribeData data)
    {
        var convertedData = new MemoryStream();

        await FFMpegArguments
            .FromPipeInput(new StreamPipeSource(data.Data))
            .OutputToPipe(new StreamPipeSink(convertedData), options => options.ForceFormat("wav"))
            .ProcessAsynchronously();

        return data with { Data = convertedData };
    }
}