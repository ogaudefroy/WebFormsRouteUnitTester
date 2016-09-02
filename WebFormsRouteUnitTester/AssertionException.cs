namespace WebFormsRouteUnitTester
{
    using System;

    /// <summary>
    /// A custom exception used to make it possible to catch only assertion-related exceptions without being tied to any unit testing framework's version of an AssertionException (e.g., NUnit.Framework.AssertionException).
    /// </summary>
    public class AssertionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        internal AssertionException(string message) 
            : base(message)
        {
        }
    }
}
