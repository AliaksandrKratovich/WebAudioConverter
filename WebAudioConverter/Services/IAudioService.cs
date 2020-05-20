using System.IO;
using System.Threading.Tasks;

namespace WebAudioConverter.Services
{
    public interface IAudioService
    {
        Task ConvertToMP3(Stream stream, string directoryPath);

        Task ConvertToWMA(Stream stream, string directoryPath);

        Task<bool> GenerateSpectograms(int minFrequency, int maxFrequency);

        Task<MemoryStream> GetMp3Spectogram();

        Task<MemoryStream> GetWMASpectogram();
    }
}
