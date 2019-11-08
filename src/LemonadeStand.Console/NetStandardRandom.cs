using System;

namespace NegativeEddy.LemonadeStand.Console
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
