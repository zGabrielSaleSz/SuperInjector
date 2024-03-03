using SuperInjector.Domain;
using SuperInjector.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SuperInjector.Core
{
    public class Container : IContainer
    {
        private ISuperInjectorContext _context;
        private IInstanceBuilder _builder;

        public Container()
        {
            _context = new SuperInjectorContext();
            _builder = new SuperInjectorInstanceBuilder(_context);
            AddSingleton<IContainer, Container>(this);
        }

        public void AddSingleton<TInjection, TImplementation>(TImplementation instance) where TImplementation : TInjection
        {
            AddSingleton(typeof(TInjection), typeof(TImplementation), instance);
        }

        public void AddSingleton<TInjection>()
        {
            Type type = typeof(TInjection);
            AddSingleton(type, type);
        }

        public void AddSingleton<TInjection, TImplementation>() where TImplementation : TInjection
        {
            AddSingleton(typeof(TInjection), typeof(TImplementation));
        }

        public void AddTransient<TInjection, TImplementation>() where TImplementation : TInjection
        {
            _context.Register(Domain.InstanceLifeTime.Transient, typeof(TInjection), typeof(TImplementation));
        }

        public IEnumerable<T> GetInstances<T>()
        {
            List<T> response = new List<T>();
            var typeInjection = typeof(T);
            IEnumerable<InjectionInstance> items = _context.GetInstancessByType(typeInjection);
            foreach (InjectionInstance injectionInstance in items)
            {
                response.Add((T)GetOrBuild(injectionInstance));
            }
            return response;
        }

        public T GetInstance<T>()
        {
            Type typeInjection = typeof(T);
            IEnumerable<InjectionInstance> instances = _context.GetInstancessByType(typeInjection);
            int instancesCount = instances.Count();

            if (instancesCount == 1)
            {
                var injectionInstance = instances.First();
                return (T)GetOrBuild(injectionInstance);
            }

            if (instancesCount == 0)
            {
                throw new MissingImplementationException($"There is no instances configured for type {typeInjection.FullName}");
            }
            else
            {
                throw new SuperInjectorException($"There is {instancesCount} instances configured for type {typeInjection.FullName}. " +
                    $"Please use IEnumerable<{typeInjection.FullName}> within the constructor " +
                    $"or use {nameof(IContainer)}.{nameof(GetInstances)}<{typeInjection.FullName}>()");
            }
        }

        private void AddSingleton(Type typeInjection, Type typeImplementation, object instance = null)
        {
            _context.Register(Domain.InstanceLifeTime.Singleton, typeInjection, typeImplementation, instance);
        }

        private object GetOrBuild(InjectionInstance injectionInstance)
        {
            if (injectionInstance.IsBuilded)
            {
                return injectionInstance.Instance;
            }
            var newInstance = _builder.BuildInstance(injectionInstance.ImplementationType);
            if (injectionInstance.IsSingleton)
            {
                injectionInstance.Instance = newInstance;
            }
            return injectionInstance.Instance;
        }
    }
}
