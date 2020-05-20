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

        public static string GetConvertedDataTypeForSending(string path)
        {
            var type = GetConvertedDataType(path);

            switch (type)
            {
                case "mp3":
                    return "audio/x-mpeg3";
                    break;
                case "wma":
                    return "audio/x-ms-wma";
                    break;
                default:
                    return null;
            }
        }

        public static string GetConvertedDataType(string path)
        {
            return path.Split('.', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
    }
}
