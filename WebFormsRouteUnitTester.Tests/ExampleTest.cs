namespace WebFormsRouteUnitTester.Tests
{
    using System.Web.Routing;
    using NUnit.Framework;

    [TestFixture]
    public class ExampleTest
    {
        private RouteTester CreateRouteTester()
        {
            var routes = new RouteCollection();
            routes.Ignore("content/{*content}");
            routes.MapPageRoute("Cookies", "cookies", "~/pages/cookies.aspx");
            routes.MapPageRoute("ItemDetails", "{culture}/items/{id}", "~/pages/items/details.aspx", false, new RouteValueDictionary(new { culture = "en-US" }));
            return new RouteTester(routes);
        }

        [Test]
        public void TestInboundRoutes()
        {
            var tester = CreateRouteTester();

            tester.WithIncomingRequest("content/item")
                .ShouldBeIgnored();

            tester.WithIncomingRequest("cookies")
                .ShouldMatchPageRoute("~/pages/cookies.aspx");
            
            tester.WithIncomingRequest("de-DE/items/33")
                .ShouldMatchPageRoute("~/pages/items/details.aspx", new { culture = "de-DE", id = 33 });

            tester.WithIncomingRequest("fake").ShouldMatchNoRoute();
        }

        [Test]
        public void TestOutboundRoutes()
        {
            var tester = CreateRouteTester();

            tester.WithRouteInfo("Cookies")
                .ShouldGenerateUrl("/cookies");

            tester.WithRouteInfo("ItemDetails", new { id = 13})
                .ShouldGenerateUrl("/en-US/items/13");

            tester.WithRouteInfo("ItemDetails", new { id = 13, culture = "fr-FR"})
                .ShouldGenerateUrl("/fr-FR/items/13");
        }
    }
}
