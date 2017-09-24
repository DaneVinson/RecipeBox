using RecipeBox.Model.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace RecipeBox.Model.Models
{
    /// <summary>
    /// Class for tracking a Recipe's ingredients.
    /// </summary>
    public class Ingredient : BaseModel
    {
        /// <summary>
        /// Ingredient description.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(
                            "{0} {1} {2}",
                            this.Quantity,
                            this.Units,
                            this.Description)
                        .Trim();
        }


        /// <summary>
        /// A description for the Ingredient.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// The quantity of the Units specified.
        /// </summary>
        [NumberOrFraction]
        public string Quantity { get; set; }

        /// <summary>
        /// The Id value of the Recipe which is the Ingredient's parent.
        /// </summary>
        public int RecipeId { get; set; }

        /// <summary>
        /// The units of measurement for the Ingredient, e.g. Tablespoons or Pounds
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Units { get; set; }
    }
}
