using RecipeBox.Core;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Data.Criteria
{
    /// <summary>
    /// Criteria used to search for recipes.
    /// </summary>
    public class RecipesCriteria : SimpleCriteria<EmptyClass>
    {
        public RecipesCriteria() : base() { }

        public RecipesCriteria(string accountId) : base(accountId) { }


        public string NameFilter { get; set; }

        public int TagId { get; set; }
    }
}
