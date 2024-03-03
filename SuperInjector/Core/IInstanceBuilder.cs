using SuperInjector.Domain;
using System;

namespace SuperInjector.Core
{
    internal interface IInstanceBuilder
    {
        object BuildInstance(Type type);
    }
}