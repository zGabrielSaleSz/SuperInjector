using SuperInjectorTests.Model.Domain;

namespace SuperInjectorTests.Model
{
    public interface IBakery
    {
        bool LightOn { get; }

        IIceCream GetIceCream();

        IBakeryContext GetContext();

        void TurnLightsOn();
    }
}
