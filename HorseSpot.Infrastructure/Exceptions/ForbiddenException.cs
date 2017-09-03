using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom Forbbiden Exception
    /// </summary>
    public class ForbiddenException : Exception
    {
        public ForbiddenException() { }

        public ForbiddenException(string message) : base(message) { }
    }
}
