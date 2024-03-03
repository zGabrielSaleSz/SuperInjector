using SuperInjector.Domain;
using SuperInjector.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SuperInjector.Core
{
    internal class SuperInjectorContext : ISuperInjectorContext
    {
        private ConcurrentDictionary<Type, List<InjectionInstance>> _implementationsByInjection;

        internal SuperInjectorContext()
        {
            _implementationsByInjection = new ConcurrentDictionary<Type, List<InjectionInstance>>();
        }

        public void Register(InstanceLifeTime lifeTime, Type injectionType, Type implementationType, object instance = null)
        {
            if (instance != null && lifeTime != InstanceLifeTime.Singleton)
            {
                throw new SuperInjectorException($"You can not inject an instance in case life time specified is not {nameof(InstanceLifeTime.Singleton)}");
            }

            InjectionInstance injectionInstance = new InjectionInstance();
            injectionInstance.ImplementationType = implementationType;
            if (lifeTime == InstanceLifeTime.Singleton)
            {
                injectionInstance.IsSingleton = true;
                injectionInstance.Instance = instance;
            }
            RegisterImplementation(injectionType, injectionInstance);
            if (injectionType != implementationType)
            {
                RegisterImplementation(implementationType, injectionInstance);
            }

        }

        public IEnumerable<InjectionInstance> GetInstancessByType(Type type)
        {
            if (_implementationsByInjection.TryGetValue(type, out List<InjectionInstance> injectionInstance))
            {
                return injectionInstance;
            } 
            return Enumerable.Empty<InjectionInstance>(); 
        }

        private void RegisterImplementation(Type injectionType, InjectionInstance injectionInstance)
        {
            _implementationsByInjection.AddOrUpdate(injectionType,
                            addValue: new List<InjectionInstance> { injectionInstance },
                            updateValueFactory: (currenct, currentValue) =>
                            {
                                currentValue.Add(injectionInstance);
                                return currentValue;
                            });
        }
    }
}
