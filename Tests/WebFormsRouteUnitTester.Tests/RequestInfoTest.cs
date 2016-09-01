namespace WebFormsRouteUnitTester.Tests
{
    using System;
    using System.Web;
    using System.Web.Routing;
    using NUnit.Framework;

    [TestFixture]
    public class RequestInfoTest
    {
        [Test]
        public void CanAlterContext()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);
            RequestInfo requestInfo = tester.WithIncomingRequest("/test");
            requestInfo.HttpContext.Items.Add("key", "value");

            Assert.That(() => requestInfo.ShouldMatchPageRoute("~/pages/test.aspx"), Throws.Nothing);
            Assert.That(requestInfo.HttpContext.Items["key"], Is.EqualTo("value"));
        }

        [Test]
        public void ShouldBeIgnored_IsIgnored_ShouldWorkFine()
        {
            var routeCollection = new RouteCollection();
            routeCollection.Ignore("ignore");

            var tester = new RouteTester(routeCollection);

            Assert.That(() => tester.WithIncomingRequest("/ignore").ShouldBeIgnored(), Throws.Nothing);
        }

        [Test]
        public void ShouldBeIgnored_UrlNotIgnored_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);

            Assert.That(
                () => tester.WithIncomingRequest("/test").ShouldBeIgnored(),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"The request was not ignored (for url: ""/test"")."));
        }

        [Test]
        public void ShouldMatchNoRoute_NoMatch_ShouldWorkFine()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);

            Assert.DoesNotThrow(() => { tester.WithIncomingRequest("/unknown").ShouldMatchNoRoute(); }, "Requested URL should not have matched a route.");
        }

        [Test]
        public void ShouldMatchNoRoute_UrlIncorrectlyMatches_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);

            Assert.That(
                () => tester.WithIncomingRequest("/test").ShouldMatchNoRoute(),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester
                    .AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"A matching route was found (for url: ""/test"")."));
        }

        [Test]
        public void ShouldMapPageRoute_NullOrEmptyUrl_Throws()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/test");
            Assert.That(
                () => requestInfo.ShouldMatchPageRoute(null),
                Throws.InstanceOf<ArgumentNullException>());
            Assert.That(
                () => requestInfo.ShouldMatchPageRoute(string.Empty),
                Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void ShouldMapPageRoute_NoRoute_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/unkown");

            Assert.That(
                () => requestInfo.ShouldMatchPageRoute("~/pages/test.aspx"),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"No matching route was found (for url: ""/unkown"")."));
        }

        [Test]
        public void ShouldMapPageRoute_NoPageRouteHandler_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.Add(new Route("test", new FakeRouteHandler()));

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/test");

            Assert.That(
                () => requestInfo.ShouldMatchPageRoute("~/pages/test.aspx"),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"RouteHandler is not a PageRouteHandler but a ""WebFormsRouteUnitTester.Tests.RequestInfoTest+FakeRouteHandler""."));
        }

        [Test]
        public void ShouldMapPageRoute_MismatchUrl_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Test", "test", "~/pages/test.aspx");

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/test");

            Assert.That(
                () => requestInfo.ShouldMatchPageRoute("~/pages/details.aspx"),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"Resolved path is: ""~/pages/test.aspx"", expected: ""~/pages/details.aspx""."));
        }

        [Test]
        public void ShouldMapPageRoute_UnexpectedRouteValues_ShouldThrowsAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Details", "details/{id}", "~/pages/details.aspx", false, new RouteValueDictionary { { "id", 1036 } });

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/details");

            Assert.That(
                () => requestInfo.ShouldMatchPageRoute("~/pages/details.aspx"),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"Route values mismatch. Expected 0 route values, but vas: 1 route values (for url: ""/details"")."));
        }

        [Test]
        public void ShouldMapPageRoute_MissingRouteValueKey_ShouldThrowAssertionException()
        {
            var routeCollection = new RouteCollection();
            routeCollection.MapPageRoute("Details", "{culture}/details/{id}", "~/pages/details.aspx", false, new RouteValueDictionary { { "culture", "en-US" } });

            var tester = new RouteTester(routeCollection);
            var requestInfo = tester.WithIncomingRequest("/fr-FR/details/13");

            Assert.That(
                () => requestInfo.ShouldMatchPageRoute("~/pages/details.aspx", new { id = 13 }),
                Throws
                    .InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"Route values mismatch. Expected 0 route values, but vas: 1 route values (for url: ""/details"")."));
        }

        class FakeRouteHandler : IRouteHandler
        {
            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
