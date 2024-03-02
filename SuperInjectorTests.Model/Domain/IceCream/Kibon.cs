namespace SuperInjectorTests.Model
{
    public class Kibon : IIceCream
    {
        public decimal GetPrice()
        {
            return 13.02m;
        }

        public decimal GetRating()
        {
            return 3.7m;
        }
    }
}
