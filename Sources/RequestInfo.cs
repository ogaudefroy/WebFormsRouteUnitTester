namespace WebFormsRouteUnitTester
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Routing;

    /// <summary>
    /// An object which encapsulates request information.
    /// </summary>
    public class RequestInfo
    {
        private readonly string requestUrl;
        private readonly RouteCollection applicationRoutes;

        internal RequestInfo(RouteCollection applicationRoutes, string url, string httpMethod)
        {
            HttpContext = TestUtility.GetHttpContext(PrepareUrl(url), httpMethod);
            this.applicationRoutes = applicationRoutes;
            requestUrl = url;
        }

        /// <summary>
        /// The mocked HTTP context for the test to be performed.
        /// </summary>
        public HttpContextBase HttpContext { get; set; }

        /// <summary>
        /// Asserts that no routes would be matched by the given request.
        /// </summary>
        /// <exception cref="AssertionException">Thrown if any route matches the given request.</exception>
        public void ShouldMatchNoRoute()
        {
            var routeData = applicationRoutes.GetRouteData(HttpContext);

            if (routeData != null && routeData.Route != null)
            {
                throw new AssertionException(string.Format("A matching route was found (for url: \"{0}\").", requestUrl));
            }
        }

        /// <summary>
        /// Asserts that the given request is ignored by the routing system.
        /// </summary>
        /// <exception cref="AssertionException">Thrown if given request is not ignored by the routing system.</exception>
        public void ShouldBeIgnored()
        {
            var routeData = applicationRoutes.GetRouteData(HttpContext);

            if (!(routeData.RouteHandler is StopRoutingHandler))
            {
                throw new AssertionException(string.Format("The request was not ignored (for url: \"{0}\").", requestUrl));
            }
        }

        /// <summary>
        /// Asserts that the routing information supplied to the method would be matched by the given request.
        /// </summary>
        /// <param name="expectedVirtualPath">The expected virtual path.</param>
        /// <param name="expectedRouteValues">The expected route values.</param>
        public void ShouldMatchPageRoute(string expectedVirtualPath, object expectedRouteValues = null)
        {
            if (string.IsNullOrEmpty(expectedVirtualPath))
            {
                throw new ArgumentNullException("expectedVirtualPath");
            }
            RouteData routeData = applicationRoutes.GetRouteData(this.HttpContext);
            if (routeData == null)
            {
                throw new AssertionException(string.Format("No matching route was found (for url: \"{0}\").", requestUrl));
            }
            var handler = routeData.RouteHandler as PageRouteHandler;
            if (handler == null)
            {
                throw new AssertionException(string.Format("RouteHandler is not a PageRouteHandler but a \"{0}\".", routeData.RouteHandler.GetType()));
            }
            if (string.Compare(handler.VirtualPath, expectedVirtualPath, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                throw new AssertionException(string.Format("Resolved path is: \"{0}\", expected: \"{1}\".", handler.VirtualPath, expectedVirtualPath));
            }

            var actualRouteValuesDictionary = routeData.Values.ToDictionary(p => p.Key, p => p.Value);
            if (actualRouteValuesDictionary.Count > 0)
            {
                if (expectedRouteValues == null)
                {
                    throw new AssertionException(string.Format("Route values mismatch. Expected 0 route values, but vas: {0} route values (for url: \"{1}\").", actualRouteValuesDictionary.Count, requestUrl));
                }
                var expectedRouteValuesDictionary = BuildDictionary(expectedRouteValues);

                if (expectedRouteValuesDictionary.Any())
                {
                    using (var enumerator = expectedRouteValuesDictionary.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            KeyValuePair<string, object> entry = enumerator.Current;
                            if (!actualRouteValuesDictionary.ContainsKey(entry.Key))
                            {
                                throw new AssertionException(string.Format("Route values mismatch. Expected route value with key \"{0}\" was not found (for url: \"{1}\").", entry.Key, requestUrl));
                            }
                            if (!ValueCompare(actualRouteValuesDictionary[entry.Key], expectedRouteValuesDictionary[entry.Key]))
                            {
                                throw new AssertionException(string.Format("Route values mismatch. Expected: route value with key \"{0}\" and value \"{1}\", but was: route value with key \"{0}\" and value \"{2}\" (for url: \"{3}\").", new object[]
                                {
                            entry.Key,
                            expectedRouteValuesDictionary[entry.Key],
                            actualRouteValuesDictionary[entry.Key],
                            requestUrl
                                }));
                            }
                        }
                    }
                }
            }
        }

        private static string PrepareUrl(string url)
        {
            if (url.StartsWith("~/"))
            {
                return url;
            }
            if (url.StartsWith("/"))
            {
                return "~" + url;
            }
            return "~/" + url;
        }

        private bool ValueCompare(object value1, object value2)
        {
            if (value1 == null & value2 == null)
            {
                return true;
            }
            return value1 is IComparable && value2 is IComparable && StringComparer.InvariantCultureIgnoreCase.Compare(value1.ToString(), value2.ToString()) == 0;
        }

        private Dictionary<string, object> BuildDictionary(object routeValues)
        {
            return routeValues
                    .GetType()
                    .GetProperties()
                    .ToDictionary(info => info.Name, info => info.GetValue(routeValues, null));
        }
    }
}