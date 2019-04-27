using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ImageProcessor
{
    public static class ResizeImage
    {
        [FunctionName("ResizeImage")]
        public static void Run(
            [BlobTrigger("images/{name}", Connection = "AZURE_STORAGE_CONNECTION_STRING")]Stream imageStream,
            [Blob("thumbnails/{name}", FileAccess.Write, Connection = "AZURE_STORAGE_CONNECTION_STRING")]Stream thumbnailImageStream,
            string name, 
            ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {imageStream.Length} Bytes");
            Image<Rgba32> originalImage = Image.Load(imageStream);            
            originalImage.Mutate(x => x.Resize(new ResizeOptions()
            {
                Size = new Size(200, 200),
                Mode = ResizeMode.Pad
            }));
            originalImage.SaveAsJpeg(thumbnailImageStream);
        }
    }
}
