using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
            [BlobTrigger("images/{name}", Connection = "AZURE_STORAGE_CONNECTION_STRING")]
            Stream imageStream,
            [Blob("thumbnails/{name}", FileAccess.Write, Connection = "AZURE_STORAGE_CONNECTION_STRING")]
            Stream thumbnailImageStream,
            [CosmosDB("imagesdb", "images", Id = "id", ConnectionStringSetting = "COSMOS_DB", CreateIfNotExists = true)]
            out dynamic document,
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

            // call computer vision api to get a description of the image
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Environment.GetEnvironmentVariable("COMP_VISION_KEY"));

            // Request parameters. A third optional parameter is "details".
            // The Analyze Image method returns information about the following
            // visual features:
            // Categories:  categorizes image content according to a
            //              taxonomy defined in documentation.
            // Description: describes the image content with a complete
            //              sentence in supported languages.
            // Color:       determines the accent color, dominant color, 
            //              and whether an image is black & white.
            string requestParameters =
                "visualFeatures=Description&language=en";

            // Assemble the URI for the REST API method.
            string uri = Environment.GetEnvironmentVariable("COMP_VISION_URL", EnvironmentVariableTarget.Process) + "?" + requestParameters;

            HttpResponseMessage response;

            // Read the contents of the image into a byte array.           

            byte[] byteData = GetImageByteData(imageStream);

            // Add the byte array as an octet stream to the request body.
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses the "application/octet-stream" content type.
                // The other content types you can use are "application/json"
                // and "multipart/form-data".
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

                // Asynchronously call the REST API method.
                response = client.PostAsync(uri, content).Result;
            }

            // Asynchronously get the JSON response.
            string result = response.Content.ReadAsStringAsync().Result;

            
            document = new 
            {
                id = name,
                imgPath = "/images/" + name,
                thumbnailPath = "/thumbnails/" + name,
                description = result
            };
        }

        private static byte[] GetImageByteData(Stream imageStream)
        {
            using (var memoryStream = new MemoryStream())
            {
                imageStream.Seek(0, SeekOrigin.Begin);
                imageStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
