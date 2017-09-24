using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Data.Managers;
using RecipeBox.Model.Models;
using RecipeBox.Test.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace RecipeBox.Data.Models.Test
{
    [TestClass()]
    public class TagDataManagerTest
    {
        // Create
        [TestMethod()]
        public async Task CreateWithNullTagReturnsError()
        {
            // Arrange
            var mockContext = new Mock<IRecipeBoxContext>();
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.CreateAsync(null);

            // Assert
            if (result.Success) { Assert.Fail("Expected create to fail."); }
            else 
            {
                Assert.IsTrue(
                        String.Compare("Tag is null.", result.ErrorMessage) == 0, 
                        "Expected error message was not returned."); 
            }
        }

        [TestMethod()]
        public async Task CreateWithFailedSaveReturnsFailure()
        {
            // Arrange
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet(new List<Tag>());
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(0));
            var tag = TestUtility.GetDefaultTag();
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.CreateAsync(tag);

            // Assert
            if (result.Success) { Assert.Fail("Expected create to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("The Tag failed to save.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod()]
        public async Task CreateWithSuccessfulSaveReturnsSuccessWithValidTag()
        {
            // Arrange
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet(new List<Tag>());
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(1));
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var tag = TestUtility.GetDefaultTag();
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.CreateAsync(tag);

            // Assert
            if (result.Success) { Assert.IsTrue(TagsMatch(result.Model, tag), "Tag returned was not correct."); }
            else { Assert.Fail("Expected create to succeed."); }
        }


        // Delete
        [TestMethod]
        public async Task DeleteWithFailedSaveReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(tag.AccountId) { Value = tag.Id };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(0));
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.DeleteAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected delete to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Delete failed to save.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task DeleteWithInvalidAccountIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(String.Concat("X", tag.AccountId)) { Value = tag.Id };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.DeleteAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected delete to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task DeleteWithInvalidIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(tag.AccountId) { Value = -1 };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.DeleteAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected delete to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task DeleteWithNullCriteriaReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            SimpleCriteria<int> criteria = null;
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.DeleteAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected delete to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Cannot locate a tag using a null criteria.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task DeleteWithSuccessfulSaveReturnsSuccess()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(tag.AccountId) { Value = tag.Id };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(1));
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.DeleteAsync(criteria);

            // Assert
            Assert.IsTrue(result.Success, "Expected delete to succeed.");
        }


        // Read
        [TestMethod]
        public async Task ReadWithInvalidAccountIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(String.Concat("X", tag.AccountId)) { Value = tag.Id };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected read to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task ReadWithInvalidIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(tag.AccountId) { Value = -1 };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected read to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task ReadWithNullCriteriaReturnsFailure()
        {
            // Arrange
            SimpleCriteria<int> criteria = null;
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected read to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Cannot locate a tag using a null criteria.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task ReadWithSuccessfulFindReturnsSuccess()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            var criteria = new SimpleCriteria<int>(tag.AccountId) { Value = tag.Id };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadAsync(criteria);

            // Assert
            if (result.Success) { Assert.IsTrue(TagsMatch(result.Model, tag), "Tag returned was not correct."); }
            else { Assert.Fail("Expected read to succeed."); }
        }


        // ReadMany
        [TestMethod]
        public async Task ReadManyReturnsAllAccountTags()
        {
            // Arrange
            var criteria = new SimpleCriteria<EmptyClass>() { AccountId = TestUtility.DefaultAccountId };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadManyAsync(criteria);

            // Assert
            if (!result.Success) { Assert.Fail("Expected read to succeed."); }
            else
            {
                Assert.IsTrue(
                        result.Model != null && result.Model.Count == mockTags.Object.Count(t => t.AccountId == TestUtility.DefaultAccountId),
                        "Expected all tags the default account only.");
            }
        }

        [TestMethod]
        public async Task ReadManyReturnsTagsOrderdByDescription()
        {
            // Arrange
            var criteria = new SimpleCriteria<EmptyClass>() { AccountId = TestUtility.DefaultAccountId };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadManyAsync(criteria);

            // Assert
            if (!result.Success) { Assert.Fail("Expected read to succeed."); }
            if (result.Model == null || result.Model.Count < 2) { Assert.Fail("Expected a valid return list."); }
            else
            {
                var descriptions = result.Model
                                        .Select(t => t.Description)
                                        .ToArray();
                bool isOrdered = true;
                for (int i = 0; i < descriptions.Length - 1; i++)
                {
                    if (String.Compare(descriptions[i], descriptions[i + 1]) > 0)
                    {
                        isOrdered = false;
                        break;
                    }
                }
                Assert.IsTrue(isOrdered, "Expected retuned list to be ordered by Description.");
            }
        }

        [TestMethod]
        public async Task ReadManyWithNoResultQueryReturnsSuccessWithEmptyList()
        {
            // Arrange
            var criteria = new SimpleCriteria<EmptyClass>() { AccountId = String.Concat(TestUtility.DefaultAccountId, "X") };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadManyAsync(criteria);

            // Assert
            if (!result.Success) { Assert.Fail("Expected read to succeed."); }
            else
            {
                Assert.IsTrue(
                        result.Model != null && result.Model.Count == 0,
                        "Expected an empty list.");
            }
        }
        
        [TestMethod]
        public async Task ReadManyWithNullCriteriaReturnsFailure()
        {
            // Arrange
            SimpleCriteria<EmptyClass> criteria = null;
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.ReadManyAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected read to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Cannot search for tags using a null criteria.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }


        // Update
        [TestMethod]
        public async Task UpdateWithConcurrencyExceptionReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            tag.Description = "new description";
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(TestUtility.DefaultAccountId) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).ThrowsAsync(new DbUpdateConcurrencyException());

            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("The Tag could not be saved because it was out of date.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithFailedSaveReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            tag.Description = "new description";
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(TestUtility.DefaultAccountId) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(0));
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("The Tag failed to save.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithInvalidAccountIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(String.Concat(TestUtility.DefaultAccountId, "X")) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithInvalidIdReturnsFailure()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            tag.Id = -1;
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(String.Concat(TestUtility.DefaultAccountId)) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithNullCriteriaReturnsFailure()
        {
            // Arrange
            SimpleCriteria<Tag> criteria = null;
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Cannot update a tag using a null criteria.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithNullTagReturnsFailure()
        {
            // Arrange
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(TestUtility.DefaultAccountId) { Value = null };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (result.Success) { Assert.Fail("Expected update to fail."); }
            else
            {
                Assert.IsTrue(
                        String.Compare("Unable to find or access the specified Tag.", result.ErrorMessage) == 0,
                        "Expected error message was not returned.");
            }
        }

        [TestMethod]
        public async Task UpdateWithSuccessfulSaveReturnsSucess()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            tag.Description = "new description";
            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(TestUtility.DefaultAccountId) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(1));
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            Assert.IsTrue(result.Success, "Expected update to succeed.");
        }

        [TestMethod]
        public async Task UpdateWithSuccessSavesCorrectData()
        {
            // Arrange
            var tag = TestUtility.GetDefaultTag();
            tag.AccountId = String.Concat(TestUtility.DefaultAccountId, "XYZ");
            tag.Description = "new description";
            tag.Recipes = new List<Recipe>() { new Recipe() { Name = "test" } };
            tag.RowVersion = new byte[5];

            SimpleCriteria<Tag> criteria = new SimpleCriteria<Tag>(TestUtility.DefaultAccountId) { Value = tag };
            var mockContext = new Mock<IRecipeBoxContext>();
            var mockTags = TestUtility.GetTagsDbSet();
            mockContext.Setup(c => c.Tags).Returns(mockTags.Object);
            mockContext.Setup(c => c.SaveChangesAsync()).Returns(Task.FromResult(1));
            var dataManager = new TagDataManager(mockContext.Object);

            // Act
            var result = await dataManager.UpdateAsync(criteria);

            // Assert
            if (!result.Success) { Assert.Fail("Expected update to succeed."); }
            else if (result.Model == null) { Assert.Fail("Expected non-null return model."); }
            else
            {
                bool correctDataSaved = String.Compare(result.Model.Description, tag.Description) == 0 &&
                                        String.Compare(result.Model.AccountId, tag.AccountId) != 0 &&
                                        (result.Model.Recipes == null || result.Model.Recipes.Count == 0) &&
                                        result.Model.RowVersion == null;
                Assert.IsTrue(correctDataSaved, "Save did not update data correctly.");
            }
        }


        private bool TagsMatch(Tag tag1, Tag tag2)
        {
            if (tag1 == null && tag2 == null) { return true; }
            else if (tag1 == null || tag2 == null) { return false; }
            else
            {
                return String.Compare(tag1.AccountId, tag2.AccountId) == 0 &&
                        String.Compare(tag1.Description, tag2.Description) == 0 &&
                        tag1.Id == tag2.Id;
            }
        }
    }
}
