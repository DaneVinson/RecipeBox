using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RecipeBox.Core;
using RecipeBox.Core.Interfaces;
using RecipeBox.Data.Criteria;
using RecipeBox.Data.Results;
using RecipeBox.Model.Models;
using RecipeBox.Test.Core;
using RecipeBox.WebApi.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using System.Web.Http.ModelBinding;
using System.Web.Http.Routing;

namespace RecipeBox.WebApi.Test.Controllers
{
    [TestClass]
    public class TagsConrollerTest
    {
        // Delete
        [TestMethod]
        public async Task DeleteFailureReturnsNotFound()
        {
            // Arrange
            var controller = ArrangeControllerForDelete(false, true);

            // Act
            var result = await controller.DeleteTag(0);
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task DeleteSuccessReturnsNoContent()
        {
            // Arrange
            var controller = ArrangeControllerForDelete(true, true);

            // Act
            var deleteResult = await controller.DeleteTag(TestUtility.GetDefaultTag().Id);
            var response = await deleteResult.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NoContent);
        }

        [TestMethod]
        public async Task DeleteWithInvalidClaimsReturnsUnauthorized()
        {
            // Arrange
            var controller = ArrangeControllerForDelete(false, false);

            // Act
            var result = await controller.DeleteTag(TestUtility.GetDefaultTag().Id);
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }


        // Get
        [TestMethod]
        public async Task GetTagsFailureReturnsEmpty()
        {
            // Arrange
            var controller = ArrangeControllerForGetTags(false, true);

            // Act
            var tags = await controller.GetTags();

            // Assert
            Assert.IsTrue(tags != null && tags.Count() == 0);
        }

        [TestMethod]
        public async Task GetTagFailureReturnsNotFound()
        {
            // Arrange
            var controller = ArrangeControllerForGetTag(false, true);

            // Act
            var result = await controller.GetTag(0);
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task GetTagSuccessReturnsOkWithTagContent()
        {
            // Arrange
            var controller = ArrangeControllerForGetTag(true, true);

            // Act
            var result = await controller.GetTag(TestUtility.GetDefaultTag().Id);
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Tag tag;
            Assert.IsTrue(response.TryGetContentValue<Tag>(out tag), "The response did not contain Tag content.");
        }

        [TestMethod]
        public async Task GetTagsWithInvalidClaimsReturnsEmpty()
        {
            // Arrange
            var controller = ArrangeControllerForGetTags(true, false);

            // Act
            var tags = await controller.GetTags();

            // Assert
            Assert.IsTrue(tags != null && tags.Count() == 0);
        }

        [TestMethod]
        public async Task GetTagWithInvalidClaimsReturnsUnathorized()
        {
            // Arrange
            var controller = ArrangeControllerForGetTag(true, false);
            SetupControllerForPost(controller);

            // Act
            var result = await controller.GetTag(TestUtility.GetDefaultTag().Id);
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }


        // Post
        [TestMethod]
        public async Task PostFailureReturnsBadRequest()
        {
            // Arrange
            var controller = ArrangeControllerForPost(false, true);
            SetupControllerForPost(controller);

            // Act
            var result = await controller.PostTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task PostSuccessReturnsCreatedWithRoute()
        {
            // Arrange
            var controller = ArrangeControllerForPost(true, true);
            SetupControllerForPost(controller);

            // Act
            var result = await controller.PostTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);
            Assert.AreEqual(response.Headers.Location, String.Format("http://localhost/api/tags/{0}", TestUtility.GetDefaultTag().Id));
        }

        [TestMethod]
        public async Task PostWithInvalidClaimsReturnsUnathorized()
        {
            // Arrange
            var controller = ArrangeControllerForPost(true, false);
            SetupControllerForPost(controller);

            // Act
            var result = await controller.PostTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }


        // Put
        [TestMethod]
        public async Task PutFailureReturnsBadRequest()
        {
            // Arrange
            var controller = ArrangeControllerForPut(false, true);

            // Act
            var result = await controller.PutTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task PutSuccessReturnsOkWithTagContent()
        {
            // Arrange
            var controller = ArrangeControllerForPut(true, true);

            // Act
            var result = await controller.PutTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
            Tag tag;
            Assert.IsTrue(response.TryGetContentValue<Tag>(out tag), "The response did not contain Tag content.");
        }

        [TestMethod]
        public async Task PutWithInvalidClaimsReturnsUnathorized()
        {
            // Arrange
            var controller = ArrangeControllerForPut(true, false);

            // Act
            var result = await controller.PutTag(TestUtility.GetDefaultTag());
            var response = await result.ExecuteAsync(new CancellationToken());

            // Assert
            Assert.AreEqual(response.StatusCode, HttpStatusCode.Unauthorized);
        }


        // Plumbing
        private TagsController ArrangeController(IClaimsProvider claimsProvider, IDataManager<Tag> dataManager)
        {
            TagsController controller = new TagsController(claimsProvider, dataManager);
            controller.Request = new HttpRequestMessage() { Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } } };
            return controller;
        }

        private TagsController ArrangeControllerForDelete(bool dataManagerResultSuccess, bool validClaim)
        {
            var mockClaimsProvider = GetMockClaimsProvider(validClaim);

            // Mock the data manager specifiying the input success flag.
            var mockDataManager = new Mock<IDataManager<Tag>>();
            ICrudResult<EmptyClass> crudResult = new SimpleCrudResult<EmptyClass>() { Success = dataManagerResultSuccess };
            mockDataManager.Setup(m => m.DeleteAsync(It.IsAny<ICrudCriteria<int>>())).Returns(Task.FromResult(crudResult));

            // Instantiate a controller with the mocked dependencies and return.
            return ArrangeController(mockClaimsProvider.Object, mockDataManager.Object);
        }

        private TagsController ArrangeControllerForGetTag(bool dataManagerResultSuccess, bool validClaim)
        {
            var mockClaimsProvider = GetMockClaimsProvider(validClaim);

            // Mock the data manager specifiying the input success flag.
            var mockDataManager = new Mock<IDataManager<Tag>>();
            ICrudResult<Tag> crudResult = new SimpleCrudResult<Tag>() { Success = dataManagerResultSuccess };
            if (dataManagerResultSuccess) { crudResult.Model = TestUtility.GetDefaultTag(); }
            mockDataManager.Setup(m => m.ReadAsync(It.IsAny<ICrudCriteria<int>>())).Returns(Task.FromResult(crudResult));

            // Instantiate a controller with the mocked dependencies and return.
            return ArrangeController(mockClaimsProvider.Object, mockDataManager.Object);
        }

        private TagsController ArrangeControllerForGetTags(bool dataManagerResultSuccess, bool validClaim)
        {
            var mockClaimsProvider = GetMockClaimsProvider(validClaim);

            // Mock the data manager specifiying the input success flag.
            var mockDataManager = new Mock<IDataManager<Tag>>();
            ICrudResult<List<Tag>> crudResult = new SimpleCrudResult<List<Tag>>() { Success = dataManagerResultSuccess };
            if (dataManagerResultSuccess) { crudResult.Model = new List<Tag>(); }
            mockDataManager.Setup(m => m.ReadManyAsync(It.IsAny<ICrudCriteria<EmptyClass>>())).Returns(Task.FromResult(crudResult));

            // Instantiate a controller with the mocked dependencies and return.
            return ArrangeController(mockClaimsProvider.Object, mockDataManager.Object);
        }

        private TagsController ArrangeControllerForPost(bool dataManagerResultSuccess, bool validClaim)
        {
            var mockClaimsProvider = GetMockClaimsProvider(validClaim);

            // Mock the data manager specifiying the input success flag.
            var mockDataManager = new Mock<IDataManager<Tag>>();
            ICrudResult<Tag> crudResult = new SimpleCrudResult<Tag>() { Success = dataManagerResultSuccess };
            if (dataManagerResultSuccess) { crudResult.Model = TestUtility.GetDefaultTag(); }
            mockDataManager.Setup(m => m.CreateAsync(It.IsAny<Tag>())).Returns(Task.FromResult(crudResult));

            // Instantiate a controller with the mocked dependencies and return.
            return ArrangeController(mockClaimsProvider.Object, mockDataManager.Object);
        }

        private TagsController ArrangeControllerForPut(bool dataManagerResultSuccess, bool validClaim)
        {
            var mockClaimsProvider = GetMockClaimsProvider(validClaim);

            // Mock the data manager specifiying the input success flag.
            var mockDataManager = new Mock<IDataManager<Tag>>();
            ICrudResult<Tag> crudResult = new SimpleCrudResult<Tag>() { Success = dataManagerResultSuccess };
            if (dataManagerResultSuccess) { crudResult.Model = TestUtility.GetDefaultTag(); }
            mockDataManager.Setup(m => m.UpdateAsync(It.IsAny<SimpleCriteria<Tag>>())).Returns(Task.FromResult(crudResult));

            // Instantiate a controller with the mocked dependencies and return.
            return ArrangeController(mockClaimsProvider.Object, mockDataManager.Object);
        }

        private Mock<IClaimsProvider> GetMockClaimsProvider(bool validClaim)
        {
            // Mock the claims provider using the specified claim.
            var mockClaimsProvider = new Mock<IClaimsProvider>();
            Claim claim = null;
            if (validClaim) { claim = new Claim(Utility.AccountIdClaimName, TestUtility.GetDefaultTag().AccountId); }
            mockClaimsProvider.Setup(p => p.GetClaim(Utility.AccountIdClaimName)).Returns(claim);
            return mockClaimsProvider;
        }

        private void SetupControllerForPost(ApiController controller)
        {
            // Setup the controller's Request.
            var httpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(httpConfiguration);
            var httpRouteData = new HttpRouteData(
                                        httpConfiguration.Routes["DefaultApi"],
                                        new HttpRouteValueDictionary { { "controller", "tags" } });
            controller.Request = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/tags/");
            controller.Request.Properties.Add(HttpPropertyKeys.HttpConfigurationKey, httpConfiguration);
            controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, httpRouteData);
        }
    }
}
