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
        public void GoBankrupt()
        {
            string[] story = new string[]
            {
                "Y","1","","",
                 // glasses, signs, price, enter
                "100","0","1","",    // day 1, spend all the money
                "0","2","1","",      // day 2, just buy signs with the profits from day 1
                "",
            };


            var stand = new LemonadeStand.Game(new TestIO(story, _output), new ConstantRandom(0));
            stand.Init();
            _output.WriteLine("***************************************************************");
            stand.Step();
            Assert.Equal(0.30M, stand.Stands[0].Assets);

            Assert.Throws<GameOverException>(() =>
            {
                _output.WriteLine("***************************************************************");
                stand.Step();
                _output.WriteLine("***************************************************************");
                stand.Step();
            });
            Assert.Equal(0.00M, stand.Stands[0].Assets);
        }

        [Fact]
        public void BasicGameRun()
        {
            string[] story = new string[]
            {
                "Y","1","","",

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

            var stand = new LemonadeStand.Game(new TestIO(story, _output), new ConstantRandom(0));
            stand.Init();
            stand.Step();
            Assert.Equal(1.85M, stand.Stands[0].Assets);
            stand.Step();
            Assert.Equal(1.70M, stand.Stands[0].Assets);
            stand.Step();
            Assert.Equal(1.35M, stand.Stands[0].Assets);
        }
    }

    public class TestIO : IGameIO
    {
        ITestOutputHelper _output;
        public string[] Story { get; }

        public TestIO(string[] story, ITestOutputHelper output)
        {
            Story = story;
            _output = output;
        }

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