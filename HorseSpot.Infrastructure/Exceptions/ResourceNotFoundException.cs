using System;

namespace HorseSpot.Infrastructure.Exceptions
{
    /// <summary>
    /// Custom Resource Not Found Exception
    /// </summary>
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException() { }

        public ResourceNotFoundException(string message) : base(message) { }
    }
}
