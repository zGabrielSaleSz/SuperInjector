namespace SuperInjectorTests.Model.Domain.Bakery
{
    public class HyperBakery : IBakery
    {
        public bool LightOn { get; private set; }

        private readonly IIceCream _iceCream;
        private readonly IBakeryContext _bakeryContext;

        public HyperBakery(IIceCream iceCream, IBakeryContext bakeryContext) {
            _iceCream = iceCream;
            _bakeryContext = bakeryContext;
        }

        public IBakeryContext GetContext()
        {
            return _bakeryContext;
        }

        public IIceCream GetIceCream()
        {
            return _iceCream;
        }

        public void TurnLightsOn()
        {
            LightOn = true;
        }
    }
}
