# WebFormsRouteUnitTester
A library for unit testing ASP.Net routing tables for WebForms. 
Supports inbound and outbound route testing

[![Build status](https://ci.appveyor.com/api/projects/status/18fyxxswmuxiwhsm?svg=true)](https://ci.appveyor.com/project/ogaudefroy/webformsrouteunittester)
## Initialize RouteTester
```C#
var routes = new RouteCollection();
routes.Ignore("content/{*content}");
routes.MapPageRoute("Cookies", "cookies", "~/pages/cookies.aspx");
routes.MapPageRoute(
	routeName: "ItemDetails", 
	routeUrl: "{culture}/items/{id}",
	physicalFile: "~/pages/items/details.aspx", 
	checkPhysicalUrlAccess: false,
	defaults: new RouteValueDictionary(new { culture = "en-US" }));
var tester = new RouteTester(routes);
```

## Inbound route testing

 - Without parameters

```C#
tester.WithIncomingRequest("/cookies")
      .ShouldMatchPageRoute("~/Pages/General/Cookies.aspx");
```
 - With parameters
```C#
tester.WithIncomingRequest("en-US/items/33")
      .ShouldMatchPageRoute("~/pages/items/details.aspx", new { culture = "en-US", id = 33 });
```
- Ignored routes
```C#
tester.WithIncomingRequest("content/bg666")
      .ShouldBeIgnored();
```
- Missing routes
```C#
tester.WithIncomingRequest("fake")
      .ShouldMatchNoRoute();
```

## Outbound route testing
- Without parameters
```C#
tester.WithRouteInfo("Cookies")
    .ShouldGenerateUrl("/cookies");
```
- With parameters
```C#
tester.WithRouteInfo("ItemDetails", new { id = 13, culture = "fr-FR"})
    .ShouldGenerateUrl("/fr-FR/items/13");
```
- Parameter default values testing
```C#
tester.WithRouteInfo("ItemDetails", new { id = 13})
    .ShouldGenerateUrl("/en-US/items/13");
```
Inspired by [MvcRouteUnitTester](http://mvcrouteunittester.codeplex.com/) 