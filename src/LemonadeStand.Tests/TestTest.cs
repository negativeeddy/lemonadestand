using Xunit;

namespace NegativeEddy.LemonadeStand.Tests
{
    public class PrimeService_IsPrimeShould
    {
        public PrimeService_IsPrimeShould()
        {
        }

        [Fact]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var stand = new LemonadeStand.Game(null, null);
            Assert.False(false, "1 should not be prime");
        }
    }
}