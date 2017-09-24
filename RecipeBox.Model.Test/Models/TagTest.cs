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
    public class TagTest
    {
        [TestMethod()]
        public void InstanceDescriptionReturnsDescriptionOrEmptyTest()
        {
            var existingTag = new Tag() { Description = "Lunch" };
            var newTag = new Tag();

            Assert.IsTrue(existingTag.InstanceDescription.Equals(existingTag.Description));
            Assert.IsTrue(newTag.InstanceDescription.Equals(String.Empty));
        }
    }
}
