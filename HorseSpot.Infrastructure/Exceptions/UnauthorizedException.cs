using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom Unauthorized Exception
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException() { }

        public UnauthorizedException(string message) : base(message) { }
    }
}
