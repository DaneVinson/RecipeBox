using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace RecipeBox.Model.Models
{
    /// <summary>
    /// Class used for adding string tags to Recipes for filtering and categorization.
    /// </summary>
    public class Tag : BaseModel
    {
        /// <summary>
        /// Default constructor initializes Recipes navigation collections as a List
        /// </summary>
        public Tag() : base()
        {
            this.Recipes = new List<Recipe>();
        }


        /// <summary>
        /// Returns the Tag's Description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Description ?? String.Empty;
        }


        /// <summary>
        /// Owning Account Id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// A description phrase for the Tag, e.g. "Dinner" or "Dessert"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        /// <summary>
        /// The collection of Recipe objects associated with the Tag. Serialization is ignored
        /// for this property.
        /// </summary>
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
