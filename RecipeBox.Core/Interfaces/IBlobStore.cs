using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Core.Interfaces
{
    /// <summary>
    /// Interface for working with BLOB storage.
    /// </summary>
    public interface IBlobStorage
    {
        /// <summary>
        /// Method to add the input file to storage with the preferred name and return its saved name.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <returns></returns>
        Task<string> AddAsync(FileInfo sourceFile, string preferredName);

        /// <summary>
        /// Delete the file specified by path from storage.
        /// </summary>
        /// <param name="filePath"></param>
        Task DeleteAsync(string fileName);
    }
}
