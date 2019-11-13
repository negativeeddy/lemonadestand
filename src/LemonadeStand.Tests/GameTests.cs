using System;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace NegativeEddy.LemonadeStand.Tests
{
    public class GameTests
    {
        private readonly ITestOutputHelper _output;

        public GameTests(ITestOutputHelper output)
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
            stand.Step();
            Assert.Equal(1.70M, stand.Assets[0]);
            stand.Step();
            Assert.Equal(1.35M, stand.Assets[0]);
        }
    }

    public class TestIO : IGameIO
    {
        ITestOutputHelper _output;
        public TestIO(ITestOutputHelper output)
        {
            _output = output;
        }

        public string [] Story { get; set; } = new string[]
        {
            "Y",
            "1",
            "",
            "",

            "10", // 10 glasses
            "3", // 3 signs
            "5", // 5 cents/glass
            "",

            "10", // 10 glasses
            "3", // 3 signs
            "5", // 5 cents/glass
            "",

            "10", // 10 glasses
            "3", // 3 signs
            "5", // 5 cents/glass
            "",
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
            string line = Story[step++];
            _output.WriteLine(">> " + line);
            return line;
        }
    }
}