using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom Validation Exception
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException() { }

        public ValidationException(string message) : base(message) { }
    }
}
