using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RecipeBox.WebApi.Criteria
{
    public class RecipesSearchCriteria
    {
        public string NameFilter { get; set; }
        public int TagId { get; set; }
    }
}