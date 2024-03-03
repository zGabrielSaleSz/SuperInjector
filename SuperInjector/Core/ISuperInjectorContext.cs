using SuperInjector.Domain;
using System;
using System.Collections.Generic;

namespace SuperInjector.Core
{
    internal interface ISuperInjectorContext
    {
        void Register(InstanceLifeTime lifeTime, Type injectionType, Type implementationType, object existingInstance = null);

        IEnumerable<InjectionInstance> GetInstancessByType(Type type);
    }
}
