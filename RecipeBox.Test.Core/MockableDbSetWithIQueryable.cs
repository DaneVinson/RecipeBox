using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Test.Core
{
    /// <summary>
    /// Found in this article http://msdn.microsoft.com/en-us/data/dn314429
    /// Works around an issue with Moq that won't let the explicit implementation of IQueryable<T> on DbSet
    /// be configured even when using As().
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MockableDbSetWithIQueryable<T> : DbSet<T>, IQueryable<T>, IDbAsyncEnumerable<T>
        where T : class
    {
        public abstract IEnumerator<T> GetEnumerator();
        public abstract Expression Expression { get; }
        public abstract Type ElementType { get; }
        public abstract IQueryProvider Provider { get; }

        IDbAsyncEnumerator<T> IDbAsyncEnumerable<T>.GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.GetEnumerator());       
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.GetEnumerator());
        }
    }
}
