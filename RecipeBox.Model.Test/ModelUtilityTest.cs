using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeBox.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RecipeBox.Model.Test
{
    [TestClass()]
    public class ModelUtilityTest
    {
        [TestMethod()]
        public void GetDecimalDisplayTest()
        {
            Assert.IsTrue(ModelUtility.GetNumericDisplay(0M).Equals("0"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.0M).Equals("1"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(-1.0M).Equals("-1"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.1M).Equals("1.1"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.18M).Equals("1.18"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.181M).Equals("1.18"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.185M).Equals("1.18"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1.1855M).Equals("1.19"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(12.3M).Equals("12.3"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(123M).Equals("123"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1234M).Equals("1,234"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1234567M).Equals("1,234,567"));
            Assert.IsTrue(ModelUtility.GetNumericDisplay(1234567890M).Equals("1,234,567,890"));
        }
    }
}
