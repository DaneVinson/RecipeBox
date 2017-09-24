using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox.Model.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RecipeBox.Model.Models.Test
{
    [TestClass()]
    public class IngredientTest
    {
        /// <summary>
        /// Ensure that the InstanceDescription property of an Ingredient returns the expected value.
        /// </summary>
        [TestMethod()]
        public void InstanceDescriptionReturnsExpectedTest()
        {
            var ingredient = new Ingredient();
            string expected = String.Format(
                                    "{0} {1} {2}",
                                    ingredient.Quantity,
                                    ingredient.Units,
                                    ingredient.Description)
                                .Trim();
            Assert.IsTrue(expected.Equals(ingredient.InstanceDescription));
        }
    }
}
