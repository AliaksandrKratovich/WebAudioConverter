using System;
using System.IO;
using System.Linq;

namespace WebAudioConverter.Helpers
{
    public static class FilesHelpers
    {
        public static void ClearAllData()
        {
            PathHelpers.LastConvertedDataPath = default;
            FilesHelpers.ClearData(PathHelpers.AudioSpectrumMP3Path);
            FilesHelpers.ClearData(PathHelpers.AudioSpectrumWMAPath);
            FilesHelpers.ClearData(PathHelpers.AudioMP3Path);
            FilesHelpers.ClearData(PathHelpers.AudioWMAPath);
        }

        public static void ClearData(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetConvertedDataType(string path)
        {
            var type = path.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();

            if (type == "mp3")
            {
                return "audio/x-mpeg3";
            }
            else if (type == "wma")
            {
                return "audio/x-ms-wma";
            }

            return null;
        }
    }
}
