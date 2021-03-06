# ImageProcessor
Serverless web app in Azure for image processing based on Microsoft [tutorial](https://docs.microsoft.com/en-ca/azure/functions/tutorial-static-website-serverless-api-with-database)

The tutorial covers:
* Configure Azure Blob storage to host a static website and uploaded images.
* Upload images to Azure Blob storage using Azure Functions.
	* Azure Storage Explorer and Emulator can be used for local testing
	* The website calls the azure function **GetUploadUrl** function to get a temporary URL where to upload the image
	* The website uploads the image to **images** container from the storage account.
* Resize images using Azure Functions.
	* **ResizeImage** function is triggered when an image is uploaded and it resizes the image
	* the resized image is placed in **thumbnails** container from the storage account
* Store image metadata in Azure Cosmos DB.
	* create a CosmosDB acount (CosmosDB emulator can also be used for local testing)
	* create **imagesdb** database and **images** collection inside this database
	* when image is resized, image metadata is saved in CosmosDB using an output binding
	* HTTP Trigger function **GetImages** reads all metadatas from CosmosDB
* Use Cognitive Services Vision API to auto-generate image captions.
* Use Azure Active Directory to secure the web app by authenticating users.

