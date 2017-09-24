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
    /// Class for tracking recipes.
    /// </summary>
    public class Recipe : BaseModel
    {
        /// <summary>
        /// Default constructor initializes navigation collections as Lists
        /// </summary>
        public Recipe() : base()
        {
            this.Ingredients = new List<Ingredient>();
            this.Tags = new List<Tag>();
        }


        /// <summary>
        /// Recipe description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                            "{0} ({1} mins prep time, serves {2})",
                            this.Name,
                            this.PreparationMinutes,
                            ModelUtility.GetNumericDisplay(this.Servings))
                        .Trim();
        }


        /// <summary>
        /// Owning Account Id.
        /// </summary>
        public string AccountId { get; set; }

        /// <summary>
        /// A description of the Recipe.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The recipe's directions.
        /// </summary>
        public string Directions { get; set; }

        /// <summary>
        /// Get/set the name of the image file associated with the Recipe.
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// The collection of Ingredient objects used in the Recipe.
        /// </summary>
        public virtual ICollection<Ingredient> Ingredients { get; set; }

        /// <summary>
        /// The name of the recipe.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Additional notes about the Recipe, e.g. its origin or alternative preparation ideas.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The approximate time in minutes it will take to prepare the Recipe.
        /// </summary>
        public int PreparationMinutes { get; set; }

        /// <summary>
        /// The approximate number of servings that the Recipe will produce.
        /// </summary>
        public double Servings { get; set; }

        /// <summary>
        /// The source of the recipe.
        /// </summary>
        [MaxLength(100)]
        public string Source { get; set; }

        /// <summary>
        /// The collection of Tag objects associted with the Recipe.
        /// </summary>
        public virtual ICollection<Tag> Tags { get; set; }
    }
}
