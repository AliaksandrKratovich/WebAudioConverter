using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using WebAudioConverter.Helpers;

namespace WebAudioConverter.Services
{
    public class AudioService : IAudioService
    {
        public async Task ConvertToMP3(Stream stream, string directoryPath)
        {
            using (var reader = new StreamMediaFoundationReader(stream))
            {
                var path = Path.Combine(directoryPath, "audio.mp3");
                MediaFoundationEncoder.EncodeToMp3(reader, path);

                PathHelpers.LastConvertedDataPath = path;

                var savePath = Path.Combine(PathHelpers.AudioWMAPath, "audio.wma");
                await SaveInputData(stream, savePath);
            }
        }

        public async Task ConvertToWMA(Stream stream, string directoryPath)
        {
            using (var reader = new StreamMediaFoundationReader(stream))
            {
                var path = Path.Combine(directoryPath, "audio.wma");
                MediaFoundationEncoder.EncodeToWma(reader, path);

                PathHelpers.LastConvertedDataPath = path;

                var savePath = Path.Combine(PathHelpers.AudioMP3Path, "audio.mp3");
                await SaveInputData(stream, savePath);
            }
        }

        private async Task SaveInputData(Stream stream, string path)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            stream.Position = 0;
            using (var writer = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, Length);
                while (bytesRead > 0)
                {
                    await writer.WriteAsync(buffer, 0, bytesRead);
                    bytesRead = await stream.ReadAsync(buffer, 0, Length);
                }
            }
        }

        public async Task<bool> GenerateSpectograms(int minFrequency, int maxFrequency)
        {
            try
            {
                var pathToMp3 = Path.Combine(PathHelpers.AudioMP3Path, "audio.mp3");
                var bytes = await GetBytesFromMp3(pathToMp3);
                var pathToMp3Spectogram = Path.Combine(PathHelpers.AudioSpectrumMP3Path, "mp3.png");
                await GenerateAndSaveSpectogram(minFrequency, maxFrequency, bytes, pathToMp3Spectogram);

                var pathToWMA = Path.Combine(PathHelpers.AudioWMAPath, "audio.wma");
                bytes = await GetBytesFromWMA(pathToWMA);
                var pathToWMASpectogram = Path.Combine(PathHelpers.AudioSpectrumWMAPath, "WMA.png");
                await GenerateAndSaveSpectogram(minFrequency, maxFrequency, bytes, pathToWMASpectogram);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task<byte[]> GetBytesFromWMA(string path)
        {
            var reader = new WMAFileReader(path);
            int bytesToRead = (int)reader.Length;
            byte[] bytes = new byte[bytesToRead];
            await reader.ReadAsync(bytes, 0, bytesToRead);
            return bytes;
        }

        private async Task<byte[]> GetBytesFromMp3(string path)
        {
            var reader = new Mp3FileReader(path);
            int bytesToRead = (int)reader.Length;
            byte[] bytes = new byte[bytesToRead];
            await reader.ReadAsync(bytes, 0, bytesToRead);
            return bytes;
        }

        private Task GenerateAndSaveSpectogram(int minFrequency, int maxFrequency, byte[] bytes, string pathToSaveSpectogram)
        {
            return Task.Run(() =>
            {
                var spectrum = new Spectrogram.Spectrogram(sampleRate: 44100, fftSize: 32768);
                float[] values = Spectrogram.Tools.FloatsFromBytesINT16(bytes);
                spectrum.AddExtend(values);
                Bitmap bmp = spectrum.GetBitmap(intensity: 1.5, freqLow: minFrequency, freqHigh: maxFrequency, showTicks: true, tickSpacingHz: 50, tickSpacingSec: 1);
                spectrum.SaveBitmap(bmp, pathToSaveSpectogram);
            });
        }

        public async Task<MemoryStream> GetMp3Spectogram()
        {
            var path = Path.Combine(PathHelpers.AudioSpectrumMP3Path, "mp3.png");

            return await GetFileByPath(path);
        }

        public async Task<MemoryStream> GetWMASpectogram()
        {
            var path = Path.Combine(PathHelpers.AudioSpectrumWMAPath, "wma.png");

            return await GetFileByPath(path);
        }

        private async Task<MemoryStream> GetFileByPath(string path)
        {
            var memory = new MemoryStream();

            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            return memory;
        }
    }
}
