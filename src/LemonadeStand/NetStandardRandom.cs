using System;

namespace NegativeEddy.LemonadeStand
{
    public class NetStandardRandom : IRandom
    {
        Random rand = new Random();

        public int Next(int maxExclusive)
        {
            return rand.Next(maxExclusive);
        }
    }
}
