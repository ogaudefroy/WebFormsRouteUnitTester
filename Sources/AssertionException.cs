namespace WebFormsRouteUnitTester
{
    using System;

    /// <summary>
    /// A custom exception used to make it possible to catch only assertion-related exceptions without being tied to any unit testing framework's version of an AssertionException (e.g., NUnit.Framework.AssertionException).
    /// </summary>
    public class AssertionException : Exception
    {
        internal AssertionException(string message) : base(message)
        {
        }
    }
}
