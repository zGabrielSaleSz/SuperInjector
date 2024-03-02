using SuperInjectorTests.Model.Domain;

namespace SuperInjectorTests.Model
{
    public class SuperBakery : IBakery
    {
        private readonly IIceCream _iceCream;
        private readonly IBakeryContext _bakeryContext;

        public SuperBakery(IIceCream iceCream, IBakeryContext bakeryContext)
        {
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
    }
}
