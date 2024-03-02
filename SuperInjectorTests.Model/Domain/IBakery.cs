using SuperInjectorTests.Model.Domain;

namespace SuperInjectorTests.Model
{
    public interface IBakery
    {
        IIceCream GetIceCream();

        IBakeryContext GetContext();
    }
}
