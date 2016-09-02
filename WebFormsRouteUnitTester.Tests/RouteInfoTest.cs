namespace WebFormsRouteUnitTester.Tests
{
    using System;
    using System.Web.Routing;
    using NUnit.Framework;

    [TestFixture]
    public class RouteInfoTest
    {
        [Test]
        public void CanAlterHttpContext()
        {
            var routes = new RouteCollection();
            routes.MapPageRoute("Details", "details", "~/pages/details.aspx");

            var tester = new RouteTester(routes);
            var routeInfo = tester.WithRouteInfo("Details");
            var ctx = routeInfo.HttpContext;

            ctx.Items.Add("key", "value");

            Assert.That(() => { routeInfo.ShouldGenerateUrl("/details"); }, Throws.Nothing);
            Assert.AreEqual("value", routeInfo.HttpContext.Items["key"]);
        }

        [Test]
        public void ShouldGenerateUrl_NullOrEmptyExpectedUrl_Throws()
        {
            var routes = new RouteCollection();
            routes.MapPageRoute("Details", "details", "~/pages/details.aspx");

            var tester = new RouteTester(routes);

            Assert.That(() => tester.WithRouteInfo("Details").ShouldGenerateUrl(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => tester.WithRouteInfo("Details").ShouldGenerateUrl(string.Empty), Throws.InstanceOf<ArgumentNullException>());
        }

        [Test]
        public void ShouldGenerateUrl_MismatchUrls_Throws()
        {
            var routes = new RouteCollection();
            routes.MapPageRoute("Details", "details", "~/pages/details.aspx");

            var tester = new RouteTester(routes);

            Assert.That(
                () => tester.WithRouteInfo("Details").ShouldGenerateUrl("/wrong"),
                Throws.InstanceOf<WebFormsRouteUnitTester.AssertionException>()
                    .With
                    .Message
                    .EqualTo(@"URL mismatch. Expected: ""/wrong"", but was: ""/details""."));
        }
    }
}