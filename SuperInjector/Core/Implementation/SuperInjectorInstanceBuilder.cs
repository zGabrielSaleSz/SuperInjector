using SuperInjector.Domain;
using SuperInjector.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperInjector.Core
{
    internal class SuperInjectorInstanceBuilder : IInstanceBuilder
    {
        private readonly ISuperInjectorContext _context;

        internal SuperInjectorInstanceBuilder(ISuperInjectorContext context) {
            _context = context;
        }

        public object BuildInstance(Type type)
        {
            if (TryBuildImplementation(type, out var result))
            {
                return result;
            }
            throw new SuperInjectorException($"Couldn't create instance of {type.FullName}");
        }

        private bool TryBuildImplementation(Type instanceType, out object result)
        {
            if (HasParameterlessConstructor(instanceType))
            {
                result = Build(instanceType);
                return true;
            }
            List<object> parametersImplementations = new List<object>();
            var constructors = instanceType.GetConstructors();
            foreach (var constructor in constructors)
            {
                parametersImplementations.Clear();
                var constructorParameters = constructor.GetParameters();
                foreach (var parameter in constructorParameters)
                {
                    if (!TryBuildInjection(parameter.ParameterType, out object parameterBuilded))
                    {
                        parametersImplementations.Clear();
                        continue;
                    }
                    parametersImplementations.Add(parameterBuilded);
                }

                if (constructorParameters.Length == parametersImplementations.Count)
                {
                    result = Build(instanceType, parametersImplementations.ToArray());
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            return TryBuildInjection(instanceType, out result);
        }

        private bool TryBuildInjection(Type typeInjection, out object result)
        {
            result = null;
            IEnumerable<InjectionInstance> existing = _context.GetInstancessByType(typeInjection);
            if (existing.Count() == 1)
            {
                InjectionInstance item = existing.FirstOrDefault();
                if (!item.IsBuilded)
                {
                    if (TryBuildImplementation(item.ImplementationType, out var newInstance))
                    {
                        if (item.IsSingleton)
                        {
                            item.Instance = newInstance;
                        }
                        result = newInstance;
                        return true;
                    }
                    return false;
                } 

                result = item.Instance;
                return true;
            }
            return false;
        }

        private bool HasParameterlessConstructor(Type result)
        {
            return result.GetConstructors().Where(c => c.GetParameters().Length == 0).Any();
        }

        private object Build(Type typeImplementation, params object[] constructorInstances)
        {
            object instance;
            if (constructorInstances.Length == 0)
            {
                instance = Activator.CreateInstance(typeImplementation);
            }
            else
            {
                instance = Activator.CreateInstance(typeImplementation, constructorInstances.ToArray());
            }
            return instance;
        }
    }
}
