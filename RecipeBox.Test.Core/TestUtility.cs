using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecipeBox.Model.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeBox.Test.Core
{
    public static class TestUtility
    {
        public static Tag GetDefaultTag()
        {
            return new Tag()
            {
                AccountId = TestUtility.DefaultAccountId,
                Description = "Test Tag",
                Id = 23
            };
        }

        public static Mock<MockableDbSetWithIQueryable<Tag>> GetTagsDbSet2(IEnumerable<Tag> tags = null)
        {
            if (tags == null)
            {
                var tag = TestUtility.GetDefaultTag();
                tags = new List<Tag>()
                {
                    new Tag() { AccountId = tag.AccountId, Description = "Tag C", Id = 1 },
                    tag,
                    new Tag() { AccountId = "X", Description = "Tag A", Id = 2 },
                    new Tag() { AccountId = tag.AccountId, Description = "Tag B", Id = 3 }
                };
            }
            var queryableTags = tags.AsQueryable();

            var mockTags = new Mock<MockableDbSetWithIQueryable<Tag>>();
            mockTags.Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Tag>(queryableTags.Provider));
            mockTags.Setup(m => m.Expression).Returns(queryableTags.Expression);
            mockTags.Setup(m => m.ElementType).Returns(queryableTags.ElementType);
            mockTags.Setup(m => m.GetEnumerator()).Returns(queryableTags.GetEnumerator());

            return mockTags;
        }

        /// <summary>
        /// Method to get a mock DbSet of Tags using the input collection of Tag objects. If no
        /// collection is passed a default one containing several Tag objects will be used.
        /// </summary>
        /// <param name="tags">The collection of Tag objects to include in the retuned set or null
        /// for the test classes standard set.</param>
        /// <returns></returns>
        public static Mock<DbSet<Tag>> GetTagsDbSet(IEnumerable<Tag> tags = null)
        {
            if (tags == null)
            {
                var tag = TestUtility.GetDefaultTag();
                tags = new List<Tag>()
                {
                    new Tag() { AccountId = tag.AccountId, Description = "Tag C", Id = 1 },
                    tag,
                    new Tag() { AccountId = "X", Description = "Tag A", Id = 2 },
                    new Tag() { AccountId = tag.AccountId, Description = "Tag B", Id = 3 }
                };
            }
            var queryableTags = tags.AsQueryable();

            var mockTags = new Mock<DbSet<Tag>>();

            mockTags.As<IDbAsyncEnumerable<Tag>>().Setup(m => m.GetAsyncEnumerator()).Returns(new TestDbAsyncEnumerator<Tag>(queryableTags.GetEnumerator()));
            mockTags.As<IQueryable<Tag>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Tag>(queryableTags.Provider));
            mockTags.As<IQueryable<Tag>>().Setup(m => m.Expression).Returns(queryableTags.Expression);
            mockTags.As<IQueryable<Tag>>().Setup(m => m.ElementType).Returns(queryableTags.ElementType);
            mockTags.As<IQueryable<Tag>>().Setup(m => m.GetEnumerator()).Returns(queryableTags.GetEnumerator());

            return mockTags;
        }

        public static readonly string DefaultAccountId = "TestingAccountId";
    }
}
