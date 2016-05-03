using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PartsUnlimited.Controllers;
using PartsUnlimited.Models;
using PartsUnlimited.UnitTests.Fakes;
using PartsUnlimited.Utils;
using PartsUnlimited.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace PartsUnlimited.UnitTests.Controllers
{
    [TestClass]
    public class OrdersControllerTests
    {
        [TestMethod]
        public async Task Order_Index()
        {
            // arrange
            var fakeDb = new FakeDataContext();
            var fakeModel = new OrdersModel(fakeDb.Orders.ToList(), "bob", new DateTimeOffset(), new DateTimeOffset(), null, false);

            var fakeOrdersQuery = new Mock<IOrdersQuery>();
            fakeOrdersQuery.Setup(o => o.IndexHelperAsync("bob", null, null, null, false))
                .ReturnsAsync(fakeModel);

            var fakeTelemetryProvider = new Mock<ITelemetryProvider>();
            OrdersController controller = GetOrdersController(fakeOrdersQuery, fakeTelemetryProvider);

            // act
            var resultTask = await controller.Index(null, null, null);
            var viewResult = resultTask as ViewResult;

            // assert
            Assert.IsNotNull(viewResult);
            fakeOrdersQuery.Verify(o => o.IndexHelperAsync("bob", null, null, null, false), Times.Once, "IndexHelperAsync not called correctly");
            var model = viewResult.Model as OrdersModel;
            Assert.IsNotNull(model);
            Assert.AreSame(model, fakeModel);
        }

        [TestMethod]
        public async Task Order_DetailWithNullId()
        {
            // arrange
            var fakeOrdersQuery = new Mock<IOrdersQuery>();
            var fakeTelemetryProvider = new Mock<ITelemetryProvider>();
            fakeTelemetryProvider.Setup(t => t.TrackTrace("Order/Server/NullId"));
            var queryString = new NameValueCollection();
            queryString.Add("id", null);
            var controller = GetOrdersController(fakeOrdersQuery, fakeTelemetryProvider, "bob", queryString);

            // act
            var resultTask = await controller.Details(null);
            var redirect = resultTask as RedirectToRouteResult;

            // assert
            Assert.IsTrue(redirect.RouteValues.Any(v => v.Key == "action" && v.Value.ToString() == "Index"));
            Assert.IsTrue(redirect.RouteValues.Any(v => v.Key == "invalidOrderSearch" && v.Value == null));
            fakeTelemetryProvider.Verify(t => t.TrackTrace("Order/Server/NullId"), Times.Once);
        }

        [TestMethod]
        public async Task Order_DetailWithUserMismatch()
        {
            // arrange
            var order = new Order()
            {
                Username = "bob",
                OrderId = 1
            };
            var fakeOrdersQuery = new Mock<IOrdersQuery>();
            fakeOrdersQuery.Setup(o => o.FindOrderAsync(1))
                .ReturnsAsync(order);

            var fakeTelemetryProvider = new Mock<ITelemetryProvider>();
            fakeTelemetryProvider.Setup(t => t.TrackTrace("Order/Server/UsernameMismatch"));
            var controller = GetOrdersController(fakeOrdersQuery, fakeTelemetryProvider, "ted");

            // act
            var resultTask = await controller.Details(1);
            var redirect = resultTask as RedirectToRouteResult;

            // assert
            Assert.IsTrue(redirect.RouteValues.Any(v => v.Key == "action" && v.Value.ToString() == "Index"));
            Assert.IsTrue(redirect.RouteValues.Any(v => v.Key == "invalidOrderSearch" && v.Value.ToString() == "1"));
            fakeTelemetryProvider.Verify(t => t.TrackTrace("Order/Server/UsernameMismatch"), Times.Once);
        }

        [TestMethod]
        public async Task Order_DetailWithNoDetails()
        {
            // arrange
            var order = new Order()
            {
                Username = "bob",
                OrderId = 1
            };
            var fakeOrdersQuery = new Mock<IOrdersQuery>();
            fakeOrdersQuery.Setup(o => o.FindOrderAsync(1))
                .ReturnsAsync(order);

            var props = new Dictionary<string, string>()
            {
                { "Id", "1" },
                { "Username", "bob" }
            };

            var fakeTelemetryProvider = new Mock<ITelemetryProvider>();
            fakeTelemetryProvider.Setup(t => t.TrackEvent("Order/Server/NullDetails", props, null));
            var controller = GetOrdersController(fakeOrdersQuery, fakeTelemetryProvider, "bob");

            // act
            var resultTask = await controller.Details(1);
            var viewResult = resultTask as ViewResult;

            // assert
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as OrderDetailsViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(0.ToString("C"), model.OrderCostSummary.CartSubTotal);
            Assert.AreEqual(0.ToString("C"), model.OrderCostSummary.CartShipping);
            Assert.AreEqual(0.ToString("C"), model.OrderCostSummary.CartTax);
            Assert.AreEqual(0.ToString("C"), model.OrderCostSummary.CartTotal);
            Assert.AreSame(order, model.Order);
            fakeTelemetryProvider.Verify(t => t.TrackEvent("Order/Server/NullDetails", props, null), Times.Once);
        }

        [TestMethod]
        public async Task Order_DetailWithOrderDetails()
        {
            // arrange
            var order = new Order()
            {
                Username = "bob",
                OrderId = 1,
                OrderDetails = new List<OrderDetail>()
                {
                    new OrderDetail()
                    {
                        Quantity = 2,
                        Product = new Product()
                        {
                            Price = 10
                        }
                    },
                    new OrderDetail()
                    {
                        Quantity = 3,
                        Product = new Product()
                        {
                            Price = 5
                        }
                    }
                }
            };
            var fakeOrdersQuery = new Mock<IOrdersQuery>();
            fakeOrdersQuery.Setup(o => o.FindOrderAsync(1))
                .ReturnsAsync(order);

            var props = new Dictionary<string, string>()
            {
                { "Id", "1" },
                { "Username", "bob" }
            };
            var measures = new Dictionary<string, double>()
            {
                { "LineItemCount", 2 }
            };

            var fakeTelemetryProvider = new Mock<ITelemetryProvider>();
            fakeTelemetryProvider.Setup(t => t.TrackEvent("Order/Server/Details", props, measures));
            var controller = GetOrdersController(fakeOrdersQuery, fakeTelemetryProvider, "bob");

            // act
            var resultTask = await controller.Details(1);
            var viewResult = resultTask as ViewResult;

            // assert
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as OrderDetailsViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual(35.ToString("C"), model.OrderCostSummary.CartSubTotal);
            Assert.AreEqual(25.ToString("C"), model.OrderCostSummary.CartShipping);
            Assert.AreEqual(3.ToString("C"), model.OrderCostSummary.CartTax);
            Assert.AreEqual(63.ToString("C"), model.OrderCostSummary.CartTotal);
            Assert.AreSame(order, model.Order);
            fakeTelemetryProvider.Verify(t => t.TrackEvent("Order/Server/Details", props, measures), Times.Once);
        }

        private static OrdersController GetOrdersController(Mock<IOrdersQuery> fakeOrdersQuery, Mock<ITelemetryProvider> fakeTelemetryProvider,
            string username = "bob", NameValueCollection queryString = null)
        {
            var controller = new OrdersController(fakeOrdersQuery.Object, fakeTelemetryProvider.Object);
            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(username, queryString), new RouteData())
            };
            return controller;
        }
    }
}