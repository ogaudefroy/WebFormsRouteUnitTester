namespace WebFormsRouteUnitTester.Tests
{
    using System;
    using System.Web.Routing;
    using NUnit.Framework;

    [TestFixture]
    public class RouteTesterTest
    {
        [Test]
        public void Constructor_NullRouteCollection_ShouldThrowArgumentNullException()
        {
            Assert.That(() => new RouteTester(null), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void Constructor_EmptyRouteCollection_ShouldThrowArgumentNullException()
        {
            Assert.That(
                () => new RouteTester(new RouteCollection()), 
                Throws
                    .ArgumentException
                    .With
                    .Message
                    .StartsWith("There are no routes in the RouteCollection."));
        }

        [Test]
        public void WithRouteInfo_NullOrEmptyRouteName_ShouldThrowArgumentNullException([Values(null, "")] string value)
        {
            var routes = new RouteCollection();
            routes.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routes);

            Assert.That(() => tester.WithRouteInfo(value), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void WithIncomingRequest_NullOrEmptyUrl_ShouldThrowArgumentNullException([Values(null, "")] string value)
        {
            var routes = new RouteCollection();
            routes.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routes);

            Assert.That(
                () => tester.WithIncomingRequest(value), 
                Throws
                    .ArgumentException
                    .With
                    .Message
                    .StartsWith("Url cannot be null or empty."));
        }
    }
}
