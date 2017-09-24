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
    public interface ICrudCriteria<T>
    {
        /// <summary>
        /// User's unique account Id for operations that require it.
        /// </summary>
        string AccountId { get; }

        /// <summary>
        /// The primary value object for the CRUD request.
        /// </summary>
        T Value { get; }
    }
}
