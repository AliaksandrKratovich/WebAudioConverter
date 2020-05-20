using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using WebAudioConverter.Helpers;
using WebAudioConverter.Services;

namespace WebAudioConverter.Controllers
{
    public class HomeController : Controller
    {
        private static bool _spectrogramsAreReady;

        private readonly IAudioService _audioService;

        public HomeController(IAudioService audioService)
        {
            this._audioService = audioService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadAudio(IFormFile uploadedAudio, int minFrequency = 1, int maxFrequency = 22000)
        {

            ValidateFrequency(ref minFrequency, ref maxFrequency);
            if (uploadedAudio == null)
            {
                return RedirectToAction("Index");
            }

            _spectrogramsAreReady = false;

            FilesHelpers.ClearAllData();

            var contentType = uploadedAudio.ContentType;
            using (var readStream = uploadedAudio.OpenReadStream())
            {
                if (contentType == "audio/mpeg")
                {
                    await _audioService.ConvertToWMA(readStream, PathHelpers.AudioWMAPath);
                }
                else if (contentType == "audio/x-ms-wma")
                {
                    await _audioService.ConvertToMP3(readStream, PathHelpers.AudioMP3Path);
                }
                else
                {
                    return RedirectToAction("Index");
                }

                _spectrogramsAreReady = await _audioService.GenerateSpectograms(minFrequency, maxFrequency);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExportAudio()
        {
            var memory = new MemoryStream();
            var convertedDataPath = PathHelpers.LastConvertedDataPath;

            if (convertedDataPath == null)
            {
                return RedirectToAction("Index");
            }

            using (var stream = new FileStream(convertedDataPath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            Response.Headers.Add("content-disposition", "attachment;" + convertedDataPath + ";");
            var type = FilesHelpers.GetConvertedDataType(convertedDataPath);

            return File(memory, type);
        }

        [HttpGet]
        public IActionResult ExportMp3Spectogram()
        {
            if (!_spectrogramsAreReady)
            {
                return RedirectToAction("Index");
            }

            return PhysicalFile(Path.Combine(PathHelpers.AudioSpectrumMP3Path, "mp3.png"), "image/png");
        }

        [HttpGet]
        public IActionResult ExportWMASpectogram()
        {
            if (!_spectrogramsAreReady)
            {
                return RedirectToAction("Index");
            }

            return PhysicalFile(Path.Combine(PathHelpers.AudioSpectrumWMAPath, "wma.png"), "image/png");
        }

        public void ValidateFrequency(ref int minFrequency, ref int maxFrequency)
        {
            if (minFrequency <= 0)
            {
                minFrequency = 1;
            }
            if (minFrequency <= 0)
            {
                minFrequency = 1;
            }
            if (maxFrequency > 22000)
            {
                maxFrequency = 22000;
            }
            if (minFrequency > maxFrequency)
            {
                int temp = minFrequency;
                minFrequency = maxFrequency;
                maxFrequency = temp;
            }
        }
    }
}
