using RecipeBox.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RecipeBox.WebApi
{
    /// <summary>
    /// IBlobStorage implementation for web server local file storage.
    /// </summary>
    public class WebServerFileStore : IBlobStorage
    {
        public async Task<string> AddAsync(FileInfo sourceFile, string preferredName)
        {
            // If the requested name is in use seperate name and extension then add a counter to name until it is unique.
            string imageFileDirectory = GetRecipeImagesPath();
            string finalPath = Path.Combine(imageFileDirectory, preferredName);
            if (File.Exists(finalPath))
            {
                string[] fileNameParts = preferredName.Split('.');
                string name = String.Join(".", fileNameParts.Take(fileNameParts.Length - 1));
                string extension = String.Format(".{0}", fileNameParts.Last());
                int i = 1;
                string testPath = Path.Combine(imageFileDirectory, String.Concat(name, i++.ToString(), extension));
                while (File.Exists(testPath))
                {
                    testPath = Path.Combine(imageFileDirectory, String.Concat(name, i++.ToString(), extension));
                }
                finalPath = testPath;
            }
            var fileInfo = sourceFile.CopyTo(finalPath);
            return fileInfo.Name;
        }

        public async Task DeleteAsync(string fileName)
        {
            var currentImagePath = Path.Combine(GetRecipeImagesPath(), fileName);
            if (File.Exists(currentImagePath)) { File.Delete(currentImagePath); }
        }

        /// <summary>
        /// Get the image files directory path for the current HttpContext creating the directory if it doesn't exist.
        /// </summary>
        private string GetRecipeImagesPath()
        {
            string path = HttpContext.Current.Server.MapPath(this.RelativeImageFilesPath);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            return path;
        }


        /// <summary>
        /// Relative path of the recipe images directory.
        /// </summary>
        private readonly string RelativeImageFilesPath = "~/RecipeImages";
    }
}
