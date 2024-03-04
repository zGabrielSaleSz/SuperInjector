using SuperInjector.Domain;
using SuperInjector.Exceptions;
using System;
using System.Collections;
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
            if (TryBuildImplementation(type, out var result, new List<string>()))
            {
                return result;
            }
            throw new SuperInjectorException($"Couldn't create instance of {type.FullName}");
        }

        private bool TryBuildImplementation(Type instanceType, out object result, List<string> dependencyStack)
        {
            if (HasParameterlessConstructor(instanceType))
            {
                result = Build(instanceType);
                return true;
            }

            if (dependencyStack.Contains(instanceType.FullName))
            {
                throw new CrossDependencyException(dependencyStack);
            }
            dependencyStack.Add(instanceType.FullName);
            List<object> parametersImplementations = new List<object>();
            var constructors = instanceType.GetConstructors();
            foreach (var constructor in constructors)
            {
                parametersImplementations.Clear();
                var constructorParameters = constructor.GetParameters();
                foreach (var parameter in constructorParameters)
                {
                    if (IsEnumerable(parameter.ParameterType))
                    {
                        var enumerableType = GetGenericIEnumerables(parameter.ParameterType);
                        if (enumerableType.Count() != 1)
                        {
                            throw new SuperInjectorException($"Couldn't resolve type {parameter.ParameterType.FullName} on injection of {instanceType.FullName}");
                        }
                        object parameterBuilded = BuildEnumerableInjection(enumerableType.First().UnderlyingSystemType, dependencyStack);
                        parametersImplementations.Add(parameterBuilded);
                    }
                    else
                    {
                        if (!TryBuildSingleInjection(parameter.ParameterType, out object parameterBuilded, dependencyStack))
                        {
                            parametersImplementations.Clear();
                            continue;
                        }
                        parametersImplementations.Add(parameterBuilded);
                    }
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
            return TryBuildSingleInjection(instanceType, out result, dependencyStack);
        }

        public IList CreateGenericList(Type listType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(listType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        private bool TryBuildSingleInjection(Type typeInjection, out object result, List<string> dependencyStack)
        {
            result = null;
            var enumerableInjection = (IList)BuildEnumerableInjection(typeInjection, dependencyStack);
            if (enumerableInjection.Count == 1)
            {
                result = enumerableInjection[0];
                return true;
            }
            return false;
        }

        private object BuildEnumerableInjection(Type typeInjection, List<string> dependencyStack)
        {
            IEnumerable<InjectionInstance> existing = _context.GetInstancessByType(typeInjection);
            IList enumerableResult = CreateGenericList(typeInjection);
            foreach (var instance in existing)
            {
                if (!instance.IsBuilded)
                {
                    if (TryBuildImplementation(instance.ImplementationType, out var newInstance, dependencyStack))
                    {
                        if (instance.IsSingleton)
                        {
                            instance.Instance = newInstance;
                        }
                        enumerableResult.Add(newInstance);
                    }
                } 
                else
                {
                    enumerableResult.Add(instance.Instance);
                }
            }
            return enumerableResult;
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

        private bool IsEnumerable(Type type)
        {
            return type != typeof(string) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public IEnumerable<Type> GetGenericIEnumerables(Type enumerableType)
        {
            return enumerableType.GetGenericArguments();
        }
    }
}
