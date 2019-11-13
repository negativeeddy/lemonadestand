using System;

namespace NegativeEddy.LemonadeStand.Console
{
    class ConsoleIO : IGameIO
    {
        public Action<string> Output => text => System.Console.Write(text);

        public string GetInput() => System.Console.ReadLine();
    }
}
