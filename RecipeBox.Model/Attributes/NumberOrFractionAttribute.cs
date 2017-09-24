using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Model.Attributes
{
    /// <summary>
    /// Attribute to ensure a string property holds a valid numeric, fraction or mixed whole number and fraction value.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NumberOrFractionAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate the input property value is a valid numeric or fractional string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            // Non-strings, nulls and empty are all invalid.
            var valueString = value as string;
            if (String.IsNullOrWhiteSpace(valueString)) { return false; }

            // Trim empty spaces.
            valueString = valueString.Trim();

            // If the string can be cast to a double it's valid otherwise use the FractionRegex to match.
            double testDouble;
            return Double.TryParse(valueString, out testDouble) || 
                ModelUtility.FractionRegex.IsMatch(valueString);
        }

        /// <summary>
        /// Validate the input property value is a valid numeric or fractional string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!this.IsValid(value)) { return new ValidationResult(String.Format("The {0} field must hold a valid numeric or fractional value.", validationContext.MemberName)); }
            else { return ValidationResult.Success; }
        }
    }
}
