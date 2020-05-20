using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebAudioConverter.Helpers
{
    public static class PathHelpers
    {
        public static string AudioMP3Path  => Path.Combine(Directory.GetCurrentDirectory(), "AudioData", "MP3");

        public static string AudioWMAPath => Path.Combine(Directory.GetCurrentDirectory(), "AudioData", "WMA");

        public static string AudioDataPath => Path.Combine(Directory.GetCurrentDirectory(), "AudioData");

        public static string AudioSpectrumMP3Path => Path.Combine(Directory.GetCurrentDirectory(), "SpectrumData", "MP3");

        public static string AudioSpectrumWMAPath => Path.Combine(Directory.GetCurrentDirectory(), "SpectrumData", "WMA");

        public static string LastConvertedDataPath { get; set; }
    }
}
