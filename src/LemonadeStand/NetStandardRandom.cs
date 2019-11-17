using System;

namespace NegativeEddy.LemonadeStand
{
    public class NetStandardRandom : IRandom
    {
        private readonly Random rand = new Random();

        public int Next(int maxExclusive)
        {
            return rand.Next(maxExclusive);
        }
    }
}
