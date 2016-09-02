namespace WebFormsRouteUnitTester
{
    using System;
    using System.Linq;
    using System.Web.Routing;
    
    /// <summary>
    /// An object used to test routes.
    /// </summary>
    public class RouteTester
    {
        private readonly RouteCollection applicationRoutes;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RouteTester"/> class.
        /// </summary>
        /// <param name="routes">A <see cref="RouteCollection"/> containing the routes under test.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="routes"/> is empty.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="routes"/> is null.</exception>
        public RouteTester(RouteCollection routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes", "The RouteCollection cannot be null.");
            }
            if (routes.Count == 0)
            {
                throw new ArgumentException("There are no routes in the RouteCollection.", "routes");
            }
            applicationRoutes = routes;
        }
        
        /// <summary>
        /// Used to supply the routing information used for an outgoing route test.
        /// </summary>
        /// <param name="routeName">The name of the route.</param>
        /// <param name="routeValues">An anonymous object containing route values.</param>
        /// <exception cref="ArgumentException">Thrown when either the <paramref name="routeName"/> argument is null, empty, or contains only whitespace.</exception>
        /// <returns>A <see cref="RouteInfo"/> object.</returns>
        public RouteInfo WithRouteInfo(string routeName, object routeValues = null)
        {
            if (string.IsNullOrWhiteSpace(routeName))
            {
                throw new ArgumentNullException("routeName");
            }
            RouteValueDictionary routeValueDictionary = routeValues != null
                                                           ? new RouteValueDictionary(routeValues
                                                                                        .GetType()
                                                                                        .GetProperties()
                                                                                        .ToDictionary(p => p.Name, p => p.GetValue(routeValues, null)))
                                                           : null;
            return new RouteInfo(applicationRoutes, routeName, routeValueDictionary);
        }

        /// <summary>
        /// Used to supply the request information used for an incoming route test.
        /// </summary>
        /// <param name="url">The request URL.</param>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="url"/> argument is null, empty, or contains only whitespace.</exception>
        /// <returns>A <see cref="RequestInfo"/> object.</returns>
        public RequestInfo WithIncomingRequest(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentException("Url cannot be null or empty.", "url");
            }
            return new RequestInfo(applicationRoutes, url);
        }
    }
}
