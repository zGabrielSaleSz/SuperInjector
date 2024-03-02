using System;
using System.Collections.Generic;
using System.Text;

namespace SuperInjector.Exceptions
{
    public class MissingImplementationException : SuperInjectorException
    {
        public MissingImplementationException(string errorMessage) : base(errorMessage)
        {

        }
    }
}
