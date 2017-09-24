using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.DataContext
{
    /// <summary>
    /// DbConfiguration implementation for RecipeBoxContext.
    /// </summary>
    internal sealed class DatabaseConfiguration : DbConfiguration
    {
        public DatabaseConfiguration() : base()
        {
            SetDatabaseInitializer<RecipeBoxContext>(null);
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy(3, TimeSpan.FromSeconds(1)));
            SetManifestTokenResolver(new SqlProviderManifestTokenResolver());
        }
    }
}
