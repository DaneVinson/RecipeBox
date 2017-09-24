using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecipeBox.Model
{
    /// <summary>
    /// Static class for RecipeBox Model utility functions.
    /// </summary>
    public static class ModelUtility
    {
        // I found this expression on The Code Project in an article written by Mykola Dobrochynskyy.
        // The article was titled "Email Address Validation Using Regular Expression".
        public static readonly Regex EmailRegex = new Regex(String.Concat(
                                                            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@",
                                                            @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.",
                                                            @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|",
                                                            @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$"));

        /// <summary>
        /// Regex expression to match fraction strings, e.g. 1/2, 1 2/3, etc.
        /// </summary>
        public static readonly Regex FractionRegex = new Regex(@"^(\d+/\d+|\d+(\s\d+/\d+)?)$");

        /// <summary>
        /// Method which returns the application standard display format for an input numeric value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNumericDisplay(double value)
        {
            return value.ToString("#,###,###,##0.##");
        }

        /// <summary>
        /// Method which returns the application standard display format for an input numeric value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetNumericDisplay(decimal value)
        {
            return ModelUtility.GetNumericDisplay(Convert.ToDouble(Decimal.Round(value, 2)));
        }
    }
}
