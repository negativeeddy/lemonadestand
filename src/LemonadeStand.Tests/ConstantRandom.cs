namespace NegativeEddy.LemonadeStand.Tests
{
    public class ConstantRandom : IRandom
    {
        int _value;

        public ConstantRandom(int value)
        {
            _value = value;
        }
        public int Next(int maxExclusive)
        {
            return _value;
        }
    }
}