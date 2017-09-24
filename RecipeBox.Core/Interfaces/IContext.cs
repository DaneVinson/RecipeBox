using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeBox.Core.Interfaces
{
    /// <summary>
    /// Interface which defines context.
    /// </summary>
    public interface IContext : IDisposable
    {
        /// <summary>
        /// Get the DbEntityEntry in the context object for the input entity object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        DbEntityEntry Entry(object entity);

        /// <summary>
        /// Get the DbEntityEntry in the context object for the input TEntity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Save changes method for the context.
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Asynchronous save changes method for the context.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Asynchronous save changes method for the context with cancellation token.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Set the specified property on the input entity to the input modified flag.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="expression"></param>
        /// <param name="entity"></param>
        /// <param name="isModified"></param>
        void SetPropertyModified<TEntity>(TEntity entity, Expression<Func<TEntity, string>> expression, bool isModified = true) where TEntity : class;
    }
}
