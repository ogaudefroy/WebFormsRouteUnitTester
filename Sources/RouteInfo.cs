namespace WebFormsRouteUnitTester
{
    using System;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// An object which encapsulates routing information.
    /// </summary>
    public class RouteInfo
    {
        private readonly string routeName;
        private readonly RouteCollection applicationRoutes;
        private readonly RouteValueDictionary routeValueDictionary;

        internal RouteInfo(RouteCollection applicationRoutes, string routeName, RouteValueDictionary routeValueDictionary)
        {
            this.applicationRoutes = applicationRoutes;
            this.routeName = routeName;
            this.routeValueDictionary = routeValueDictionary;
            HttpContext = TestUtility.GetHttpContext();
        }

        /// <summary>
        /// The mocked HTTP context for the test to be performed.
        /// </summary>
        public HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Asserts that the URL supplied to the method is the URL that is generated with the given routing information.
        /// </summary>
        /// <param name="expectedUrl">The URL that is expected to be generated.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="expectedUrl"/> argument is null, empty, or contains only whitespace.</exception>
        /// <exception cref="AssertionException">Thrown when the expected URL is not the URL that is generated with the given routing information.</exception>
        public void ShouldGenerateUrl(string expectedUrl)
        {
            if (string.IsNullOrWhiteSpace(expectedUrl))
            {
                throw new ArgumentNullException("Url cannot be null or empty.", "expectedUrl");
            }
            var vpd = this.applicationRoutes.GetVirtualPath(new RequestContext(this.HttpContext, new RouteData()), routeName, this.routeValueDictionary);
            string generatedUrl = vpd != null ? vpd.VirtualPath : null;

            if (expectedUrl != generatedUrl)
            {
                throw new AssertionException(string.Format("URL mismatch. Expected: \"{0}\", but was: \"{1}\".", expectedUrl, generatedUrl));
            }
        }
    }
}
