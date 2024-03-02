using System;
using System.Collections.Generic;
using System.Text;

namespace SuperInjectorTests.Model.Domain.Bakery
{
    public class HyperBakery : IBakery
    {
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
    }
}
