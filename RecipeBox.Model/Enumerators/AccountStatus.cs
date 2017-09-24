using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Model.Enumerators
{
    /// <summary>
    /// Used to govern values of the Account model's Status property.
    /// </summary>
    public enum AccountStatus
    {
        Unconfirmed = 0,
        Active = 1,
        Suspended = 2,
        Deleted = 3
    }
}
