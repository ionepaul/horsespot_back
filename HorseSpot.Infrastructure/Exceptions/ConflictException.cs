using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException() { }

        public ConflictException(string message) : base(message) { }
    }
}
