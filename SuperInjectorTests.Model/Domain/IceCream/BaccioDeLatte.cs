namespace SuperInjectorTests.Model
{
    public class BaccioDeLatte : IIceCream
    {
        public decimal GetPrice()
        {
            return 40M;
        }

        public decimal GetRating()
        {
            return 4.6m;
        }
    }
}
