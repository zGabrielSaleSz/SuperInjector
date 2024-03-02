using SuperInjector.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperInjector.Core
{
    public class Container : IContainer
    {
        private HashSet<Type> _singletonTypes;
        private Dictionary<Type, object> _singletonInstances;
        private Dictionary<Type, HashSet<Type>> _implementationsByInjection;

        public Container()
        {
            _singletonTypes = new HashSet<Type>();
            _singletonInstances = new Dictionary<Type, object>();
            _implementationsByInjection = new Dictionary<Type, HashSet<Type>>();
            
            AddSingleton<IContainer, Container>();
        }

        public void AddSingleton<TInjection>()
        {
            Type type = typeof(TInjection);
            _singletonTypes.Add(type);
            AddImplementation(type, type);
        }

        public void AddSingleton<TInjection, TImplementation>() where TImplementation : TInjection
        {
            Type typeInjection = typeof(TInjection);
            Type typeImplementation = typeof(TImplementation);
            _singletonTypes.Add(typeInjection);
            AddImplementation(typeInjection, typeImplementation);
        }

        public void AddTransient<TInjection, TImplementation>() where TImplementation : TInjection
        {
            var typeInjection = typeof(TInjection);
            var typeImplementation = typeof(TImplementation);
            AddImplementation(typeInjection, typeImplementation);
        }

        private void AddImplementation(Type typeInjection, Type typeImplementation)
        {
            if (_implementationsByInjection.TryGetValue(typeInjection, out HashSet<Type> existing))
            {
                existing.Add(typeImplementation);
            }
            else
            {
                _implementationsByInjection.Add(typeInjection, new HashSet<Type> { typeImplementation });
            }
        }

        public IEnumerable<T> GetInstances<T>()
        {
            List<T> response = new List<T>();
            var typeInjection = typeof(T);
            if (_implementationsByInjection.TryGetValue(typeInjection, out HashSet<Type> implementations))
            {
                foreach (var implementation in implementations)
                {
                    if (!BuildImplementation(typeInjection, implementation, out object resultBuilded))
                    {
                        throw new SuperInjectorException("Couldn't build one of the instances");
                    }
                    response.Add((T)resultBuilded);
                }
            }
            return response;
        }

        public T GetInstance<T>()
        {
            Type typeInjection = typeof(T);
            if (TryBuild(typeInjection, out object result))
            {
                return (T)result;
            }
            throw new MissingImplementationException("");
        }

        private bool TryBuild(Type typeInjection, out object result)
        {
            result = null;
            if (_singletonInstances.TryGetValue(typeInjection, out object existing))
            {
                result = existing;
                return true;
            }

            if (!_implementationsByInjection.TryGetValue(typeInjection, out HashSet<Type> instancesType))
            {
                return false;
            }

            if (instancesType.Count > 1)
            {
                throw new SuperInjectorException($"Founded types {string.Join(",",instancesType.Select(i => i.FullName))} instances to inject as parameter for type {typeInjection.FullName}, please use ${nameof(GetInstances)} instead");
            }

            Type instanceType = instancesType.FirstOrDefault();
            return BuildImplementation(typeInjection, instanceType, out result);
        }

        private bool BuildImplementation(Type typeInjection, Type instanceType, out object result)
        {
            if (HasParameterlessConstructor(instanceType))
            {
                result = Build(typeInjection, instanceType);
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
                    if (!TryBuild(parameter.ParameterType, out object parameterBuilded))
                    {
                        parametersImplementations.Clear();
                        continue;
                    }
                    parametersImplementations.Add(parameterBuilded);
                }

                if (constructorParameters.Length == parametersImplementations.Count)
                {
                    result = Build(typeInjection, instanceType, parametersImplementations.ToArray());
                    return true;
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            return TryBuild(instanceType, out result);
        }

        private bool HasParameterlessConstructor(Type result)
        {
            return result.GetConstructors().Where(c => c.GetParameters().Length == 0).Any();
        }

        private object Build(Type typeInjection, Type typeImplementation, params object[] constructorInstances)
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

            if (_singletonTypes.Contains(typeInjection))
            {
                if (!_singletonInstances.ContainsKey(typeInjection))
                {
                    _singletonInstances.Add(typeInjection, instance);
                } 
                else
                {
                    var existingImplementationType = SwitchSingletonTypeInjectionToTypeImplementation(typeInjection);
                    if (existingImplementationType == typeImplementation)
                    {
                        throw new SuperInjectorException($"You can't singletons instances of the same type {typeImplementation.FullName}");
                    }
                    _singletonInstances.Add(typeImplementation, instance);
                }
            }

            return instance;
        }


        /// <param name="typeInjection"></param>
        /// <returns>returns the implementation type</returns>
        private Type SwitchSingletonTypeInjectionToTypeImplementation(Type typeInjection)
        {
            var implementation = _singletonInstances[typeInjection];
            _singletonInstances.Remove(typeInjection);
            var implementationType = implementation.GetType();
            _singletonInstances.Add(implementationType, implementation);
            return implementationType;
        }
    }
}
