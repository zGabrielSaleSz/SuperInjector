using System;

namespace SuperInjector.Exceptions
{
    public class SuperInjectorException : Exception
    {
        public SuperInjectorException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
