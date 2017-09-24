using RecipeBox.Core.Interfaces;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Data.Criteria
{
    /// <summary>
    /// Simple generic criteria object implementation of ICrudCriteria.
    /// </summary>
    public class SimpleCriteria<T> : ICrudCriteria<T>
    {
        public SimpleCriteria() { }

        public SimpleCriteria(string accountId)
        {
            this.AccountId = accountId;
        }


        public string AccountId { get; set; }

        public T Value { get; set; }
    }
}
