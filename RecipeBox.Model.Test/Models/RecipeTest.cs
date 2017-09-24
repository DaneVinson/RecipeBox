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
    public class RecipeTest
    {
        /// <summary>
        /// Ensure that the InstanceDescription property of a Recipe returns the expected value.
        /// </summary>
        [TestMethod()]
        public void InstanceDescriptionReturnsExpectedTest()
        {
            var recipe = new Recipe() { Name = null, PreparationMinutes = -1, Servings = -1 };
            var expected = String.Format(
                                    "{0} ({1} mins prep time, serves {2})",
                                    recipe.Name,
                                    recipe.PreparationMinutes,
                                    recipe.Servings)
                                .Trim();
            Assert.IsTrue(recipe.InstanceDescription.Equals(expected));
        }
    }
}
