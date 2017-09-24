using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeBox.Model.Attributes
{
    /// <summary>
    /// Attribute to ensure a string property holds a valid email address.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class EmailAddressAttribute : ValidationAttribute
    {
        /// <summary>
        /// Validate the input property value is an email address format.
        /// </summary>
        public override bool IsValid(object value)
        {
            var valueString = value as string;
            if (String.IsNullOrWhiteSpace(valueString)) { return false; }
            else { return ModelUtility.EmailRegex.IsMatch(valueString); }
        }

        /// <summary>
        /// Validate the input property value is a valid numeric or fractional string.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!this.IsValid(value)) 
            {
                return new ValidationResult(
                                String.Format(
                                    "The {0} does not hold a valid email address.", 
                                    validationContext.MemberName)); 
            }
            else { return ValidationResult.Success; }
        }
    }
}
