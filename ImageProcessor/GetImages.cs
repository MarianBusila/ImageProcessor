using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ImageProcessor
{
    public class ImageMetadata
    {
        public string id { get; set; }
        public string imgPath { get; set; }
        public string thumbnailPath { get; set; }
    }
    public static class GetImages
    {
        [FunctionName("GetImages")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(databaseName: "imagesdb", collectionName: "images", ConnectionStringSetting = "COSMOS_DB",
                SqlQuery = "select * from c order by c._ts desc")] IEnumerable<ImageMetadata> imageMetadatas,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. Number of image metadata documents in CosmosDB: {imageMetadatas.Count()}");
            return new OkObjectResult(imageMetadatas);
        }
    }
}
