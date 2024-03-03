using System;

namespace SuperInjector.Domain
{
    internal class InjectionInstance
    {
        internal bool IsBuilded => Instance != null;
        internal Type ImplementationType { get; set; }
        internal object Instance { get; set; }
        internal bool IsSingleton { get; set; }
    }
}
