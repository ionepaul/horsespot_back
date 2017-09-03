using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom Conflict Exception
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException() { }

        public ConflictException(string message) : base(message) { }
    }
}
