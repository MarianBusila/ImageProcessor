# ImageProcessor
Serverless web app in Azure for image processing based on Microsoft [tutorial](https://docs.microsoft.com/en-ca/azure/functions/tutorial-static-website-serverless-api-with-database)

The tutorial covers:
* Configure Azure Blob storage to host a static website and uploaded images.
* Upload images to Azure Blob storage using Azure Functions.
	* The website calls the azure function **GetUploadUrl** function to get a temporary URL where to upload the image
	* The website uploads the image to **images** container from the storage account.
* Resize images using Azure Functions.
	* **ResizeImage** function is triggered when an image is uploaded and it resizes the image
	* the resized image is placed in **thumbnails** container from the storage account
* Store image metadata in Azure Cosmos DB.
* Use Cognitive Services Vision API to auto-generate image captions.
* Use Azure Active Directory to secure the web app by authenticating users.

