using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace NegativeEddy.LemonadeStand.Tests
{
    public class PrimeService_IsPrimeShould
    {
        private readonly ITestOutputHelper _output;
        public PrimeService_IsPrimeShould(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BasicGameRun()
        {
            var stand = new LemonadeStand.Game(new TestIO(_output), new ConstantRandom(0));
            stand.Init();
            stand.Step();
            Assert.Equal(1.85M, stand.Assets[0]);
        }
    }

    public class TestIO : IGameIO
    {
        ITestOutputHelper _output;
        public TestIO(ITestOutputHelper output)
        {
            _output = output;
        }

        public (string input, string output)[] Story { get; set; } = new (string, string)[]
        {
            ("Y", null),
            ("1", null),
            ("", null),
            ("", null),
            ("10", null), // 10 glasses
            ("3", null), // 3 signs
            ("5", null), // 5 cents/glass
            ("", null),
        };

        public string[] ExpectedOutput { get; set; } = new string[]
        {
            "HI!  WELCOME TO LEMONSVILLE, CALIFORNIA!",
            "",
            "IN THIS SMALL TOWN, YOU ARE IN CHARGE OF",
            "RUNNING YOUR OWN LEMONADE STAND. YOU CAN",
            "COMPETE WITH AS MANY OTHER PEOPLE AS YOU",
            "WISH, BUT HOW MUCH PROFIT YOU MAKE IS UP",
            "TO YOU (THE OTHER STANDS' SALES WILL NOT",
            "AFFECT YOUR BUSINESS IN ANY WAY). IF YOU",
            "MAKE THE MOST MONEY, YOU'RE THE WINNER!!",
            "",
            "ARE YOU STARTING A NEW GAME? (YES OR NO)"
        };

        private int step = 0;
        public Action<string> Output => o => _output.WriteLine(o);

        public string GetInput()
        {
            AssertStep(step - 1);
            string line = Story[step++].input;
            _output.WriteLine(line);
            return line;
        }

        private void AssertStep(int i)
        {
            if (i < 0 || Story[i].output == null)
            {
                return;
            }
        }
    }
}