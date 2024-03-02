using SuperInjector.Core;

namespace SuperInjectorTests.Model
{
    public class DependencyInjectionManager
    {
        private readonly IContainer _container;

        public DependencyInjectionManager(IContainer container)
        {
            _container = container;
        }


        public void TurnOnBakeriesLights()
        {
            IEnumerable<IBakery> registeredBakeries = _container.GetInstances<IBakery>();

            foreach (var bakery in registeredBakeries)
            {
                bakery.TurnLightsOn();
            }
        }
    }
}
