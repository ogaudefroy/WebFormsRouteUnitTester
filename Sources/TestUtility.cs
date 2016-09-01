namespace WebFormsRouteUnitTester
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Moq;

    internal static class TestUtility
    {
        internal static HttpContextBase GetHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            var mockRequest = new Mock<HttpRequestBase>();

            string filePath = targetUrl != null && targetUrl.Contains("?")
                                 ? targetUrl.Substring(0, targetUrl.IndexOf("?", StringComparison.InvariantCulture))
                                 : targetUrl;

            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(filePath);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            var mockResponse = new Mock<HttpResponseBase>();
            mockResponse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            var mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResponse.Object);
            mockContext.Setup(m => m.Items).Returns(new Dictionary<object, object>());

            return mockContext.Object;
        }
    }
}
