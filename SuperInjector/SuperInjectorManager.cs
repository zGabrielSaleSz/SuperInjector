using SuperInjector.Core;

namespace SuperInjector
{
    public static class SuperInjectorManager
    {
        private static IContainer _container;

        public static IContainer GetContainer()
        {
            if (_container == null)
            {
                _container = new Container();
            }
            return _container;
        }
    }
}
