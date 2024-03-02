using SuperInjector.Core;
using SuperInjectorTests.Model;

namespace SuperInjector.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IContainer container = new Container();
            container.AddSingleton<IIceCream, Kibon>();
            container.AddSingleton<IBakery, SuperBakery>();

            IBakery bakery = container.GetInstance<IBakery>();
        }
    }
}