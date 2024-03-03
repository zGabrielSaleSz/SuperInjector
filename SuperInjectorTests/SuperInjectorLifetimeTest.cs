using SuperInjector.Core;
using SuperInjectorTests.Model;
using SuperInjectorTests.Model.Domain;
using SuperInjectorTests.Model.Domain.Bakery;

namespace SuperInjectorTests
{
    public class SuperInjectorLifetimeTest
    {
        [Fact]
        public void Should_UseSameInstance_When_Singleton()
        {
            IContainer container = new Container();
            container.AddSingleton<IBakeryContext, BakeryContext>();
            container.AddSingleton<IIceCream, Kibon>();
            container.AddSingleton<IBakery, SuperBakery>();

            IBakery bakery = container.GetInstance<IBakery>();
            IIceCream iceCream = container.GetInstance<IIceCream>();

            // same instance
            Assert.Equal(bakery.GetIceCream(), iceCream);
        }

        [Fact]
        public void Should_CreateInstance_When_Transient()
        {
            IContainer container = new Container();
            container.AddSingleton<IBakery, HyperBakery>();

            container.AddTransient<IBakeryContext, BakeryContext>();
            container.AddSingleton<IIceCream, Kibon>();
            container.AddSingleton<IBakery, SuperBakery>();

            var bakery = container.GetInstances<IBakery>().ToArray();

            IBakeryContext contextBakery = bakery[0].GetContext();
            IBakeryContext contextAnotherBakery = bakery[1].GetContext();

            Assert.NotEqual(contextBakery, contextAnotherBakery);
        }

        [Fact]
        public void Should_KeepSingletonByType_When_MoreThanOneImplementationByInterface()
        {
            IContainer container = new Container();
            container.AddSingleton<IBakery, SuperBakery>();
            container.AddSingleton<IBakery, HyperBakery>();
            container.AddSingleton<IIceCream, BaccioDeLatte>();
            container.AddTransient<IBakeryContext, BakeryContext>();

            var bakery = container.GetInstances<IBakery>().ToArray();

            SuperBakery superBakeryFromImplementation = container.GetInstance<SuperBakery>();
            HyperBakery hyperBakeryFromImplementation = container.GetInstance<HyperBakery>();

            var superBakeryFromGetInstances = bakery
                .Where(a => a.GetType() == typeof(SuperBakery))
                .FirstOrDefault();

            var hyperBakeryFromGetInstances = bakery
                .Where(a => a.GetType() == typeof(HyperBakery))
                .FirstOrDefault();

            Assert.NotNull(superBakeryFromImplementation);
            Assert.NotNull(hyperBakeryFromImplementation);

            Assert.Equal(superBakeryFromImplementation, superBakeryFromGetInstances);
            Assert.Equal(hyperBakeryFromImplementation, hyperBakeryFromGetInstances);
        }

        [Fact]
        public void Should_InjectContainer_When_AnyInstanceRequiresContainerAsDependency()
        {
            IContainer container = new Container();

            container.AddSingleton<IBakery, SuperBakery>();
            container.AddSingleton<IBakery, HyperBakery>();
            container.AddSingleton<IIceCream, BaccioDeLatte>();
            container.AddTransient<IBakeryContext, BakeryContext>();
            container.AddSingleton<DependencyInjectionManager>();

            DependencyInjectionManager instance = container.GetInstance<DependencyInjectionManager>();
            instance.TurnOnBakeriesLights();

            var instances = container.GetInstances<IBakery>();
            Assert.True(instances.All(l => l.LightOn == true));
        }

        [Fact]
        public void Should_AllowRegistration_After_AnyGetInstancesMethod()
        {
            IContainer container = new Container();
            container.AddTransient<IBakeryContext, BakeryContext>();
            container.AddSingleton<IIceCream, BaccioDeLatte>();

            var baccioDeLatte = container.GetInstance<IIceCream>();
            var baccioPrice = baccioDeLatte.GetPrice();
            var rating = baccioDeLatte.GetRating();

            container.AddSingleton<IBakery, SuperBakery>();

            var bakery = container.GetInstance<IBakery>();
            Assert.Equal(bakery.GetIceCream(), baccioDeLatte); 
        }

        [Fact]
        public void Should_ResolveInstance_When_UsingImplementationType()
        {
            IContainer container = new Container();
            container.AddTransient<IBakeryContext, BakeryContext>();
            container.AddSingleton<IIceCream, BaccioDeLatte>();

            var baccioDeLatte = container.GetInstance<IIceCream>();
            var baccioPrice = baccioDeLatte.GetPrice();
            var rating = baccioDeLatte.GetRating();

            container.AddSingleton<IBakery, SuperBakery>();

            var bakery = container.GetInstance<SuperBakery>();
            var bakeryInjection = container.GetInstance<IBakery>();

            Assert.Equal(bakery.GetIceCream(), baccioDeLatte);
            Assert.Equal(bakeryInjection, bakery);
        }
    }
}