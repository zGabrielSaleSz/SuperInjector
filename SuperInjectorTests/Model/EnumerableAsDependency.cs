namespace SuperInjectorTests.Model
{
    internal class EnumerableAsDependency
    {
        private IEnumerable<IBakery> _bakeries;
        public EnumerableAsDependency(IEnumerable<IBakery> bakeries)
        {
            _bakeries = bakeries;
        }

        public void TurnOnBakeriesLights()
        {
            foreach (var bakery in _bakeries)
            {
                bakery.TurnLightsOn();
            }
        }
    }
}
