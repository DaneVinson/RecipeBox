using RecipeBox.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Model
{
    /// <summary>
    /// Base class for all data model objects.
    /// </summary>
    public abstract class BaseModel : IModel
    {
        /// <summary>
        /// A unique Id for the entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// A description for an instance of the model suitable for use in UI. Defaults to ToString.
        /// </summary>
        public virtual string InstanceDescription { get { return this.ToString(); } }

        /// <summary>
        /// A row version property for concurency.
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
