using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Model.Interfaces
{
    /// <summary>
    /// Interface which defines a basic entity.
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// Unique Id for the entity.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// A description for an instance of the model suitable for use in UI.
        /// </summary>
        string InstanceDescription { get; }

        /// <summary>
        /// A row version variable for concurency.
        /// </summary>
        byte[] RowVersion { get; set; }
    }
}
