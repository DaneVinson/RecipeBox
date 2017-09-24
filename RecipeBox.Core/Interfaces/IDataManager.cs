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
    /// Interfact for describing how the domain model should persist data CRUD operations to a data store.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDataManager<T> :IDisposable where T : class
    {
        /// <summary>
        /// Create a new model.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ICrudResult<T>> CreateAsync(T model);

        /// <summary>
        /// Delete a model.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<ICrudResult<EmptyClass>> DeleteAsync(ICrudCriteria<int> criteria);

        /// <summary>
        /// Get a single model by Id
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<ICrudResult<T>> ReadAsync(ICrudCriteria<int> criteria);

        /// <summary>
        /// Get a collection of models using a criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<ICrudResult<List<T>>> ReadManyAsync(ICrudCriteria<EmptyClass> criteria);

        /// <summary>
        /// Update a model.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        Task<ICrudResult<T>> UpdateAsync(ICrudCriteria<T> criteria);
    }
}
