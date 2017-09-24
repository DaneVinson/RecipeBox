using RecipeBox.Model;
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
    /// Interface to describe the results of a standard CRUD operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICrudResult<T> where T : class
    {
        /// <summary>
        /// Primary failure message for the CRUD operation.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Collection of result messages if applicable, e.g. multiple errors.
        /// </summary>
        List<string> Messages { get; set; }

        /// <summary>
        /// The model object returned from the CRUD operation if any.
        /// </summary>
        T Model { get; set; }

        /// <summary>
        /// Was the CRUD operation a success.
        /// </summary>
        bool Success { get; set; }
    }
}
