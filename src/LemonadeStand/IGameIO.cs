using System;

namespace NegativeEddy.LemonadeStand
{
    public interface IGameIO
    {
        public Action<string> Output { get; }

        public string GetInput();
    }
}
