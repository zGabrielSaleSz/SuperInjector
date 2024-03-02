using System;
using System.Collections.Generic;
using System.Text;

namespace SuperInjector.Core
{
    public interface IContainer
    {
        void AddSingleton<TInjection, TImplementation>() where TImplementation : TInjection;
        void AddTransient<TInjection, TImplementation>() where TImplementation : TInjection;
        T GetInstance<T>();
        IEnumerable<T> GetInstances<T>();
    }
}
