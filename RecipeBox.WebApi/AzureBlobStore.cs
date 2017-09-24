using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// IBlobStorage implementation for Azure.
    /// </summary>
    public class AzureBlobStore : IBlobStorage
    {
        #region IBlobStorage Memebers

        public async Task<string> AddAsync(FileInfo sourceFile, string preferredName)
        {
            // Retrieve a reference to a container. 
            var container = GetRecipeImagesContainer();

            // Find a unique name starting with the preffered name.
            string blobName = preferredName;
            string[] nameParts = blobName.Split('.');
            string name = String.Join(".", nameParts.Take(nameParts.Length - 1));
            string extension = nameParts.Last();
            int counter = 1;
            var blob = container.GetBlockBlobReference(blobName);
            while (await blob.ExistsAsync())
            {
                blobName = String.Format("{0}_{1}.{2}", name, counter++, extension);
                blob = container.GetBlockBlobReference(blobName);
            }

            // Create or overwrite the blob with contents from a local file.
            using (var fileStream = File.OpenRead(sourceFile.FullName))
            {
                await blob.UploadFromStreamAsync(fileStream);
            }

            // Return the final name of the blob.
            return blobName;
        }

        public async Task DeleteAsync(string fileName)
        {
            // Retrieve a reference to a container. 
            var container = GetRecipeImagesContainer();

            // Get a reference to the blob.
            var blob = container.GetBlockBlobReference(fileName);

            // Delete the blob if it exists.
            if (await blob.ExistsAsync()) { await blob.DeleteAsync(); }
        }

        #endregion

        /// <summary>
        /// Get a CloudBlobContainer for Recipe images.
        /// </summary>
        /// <returns></returns>
        private CloudBlobContainer GetRecipeImagesContainer()
        {
            // Retrieve storage account from connection string.
            string connectionString = ConfigurationManager.ConnectionStrings[CoreUtility.StorageConnectionStringKey].ConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            return blobClient.GetContainerReference(this.RecipeImagesContainerName);
        }


        /// <summary>
        /// The name of the Azure blob storage conatiner for Recipe images.
        /// </summary>
        private readonly string RecipeImagesContainerName = "recipeimages";
    }
}
