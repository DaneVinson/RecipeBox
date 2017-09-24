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
    /// Criteria used to authorize an account.
    /// </summary>
    public class AuthorizationCriteria : SimpleCriteria<EmptyClass>
    {
        public AuthorizationCriteria(string userName, string password) : base() 
        {
            this.Password = password;
            this.UserName = userName;
        }


        public string Password { get; private set; }

        public string UserName { get; private set; }
    }
}
