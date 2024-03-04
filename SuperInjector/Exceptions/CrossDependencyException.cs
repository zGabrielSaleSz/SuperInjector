using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SuperInjector.Exceptions
{
    public class CrossDependencyException : SuperInjectorException
    {
        private string _message;
        public CrossDependencyException(IEnumerable<string> dependencyStack) {
            _message = string.Concat("Detected cross dependency, sequence of dependency:\r\n ", string.Join("\r\n -> ", dependencyStack), ".");
        }

        public override string Message => _message; 
    }
}
