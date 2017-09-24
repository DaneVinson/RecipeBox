using RecipeBox.Core;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.DataContext
{
    /// <summary>
    /// IManifestTokenResolver implementation to use when connecting to Azure SQL. This prevents the addition query
    /// Entity Framework would otherwise perform automatically prior to each executed query to return the version
    /// of the SQL database engine in use.
    /// </summary>
    internal class SqlProviderManifestTokenResolver : IManifestTokenResolver
    {
        /// <summary>
        /// Default implementation to act as fallback if a SqlServerVerion is not specified in the app config.
        /// </summary>
        private readonly IManifestTokenResolver DefaultResolver = new DefaultManifestTokenResolver();

        /// <summary>
        /// Resolve the token with a string representing the SQL Server version in use.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string ResolveManifestToken(DbConnection connection)
        {
            string sqlServerVersion = CoreUtility.SqlServerVersion;
            if (String.IsNullOrWhiteSpace(sqlServerVersion)) { return this.DefaultResolver.ResolveManifestToken(connection); }
            else { return sqlServerVersion; }
        }
    }
}
