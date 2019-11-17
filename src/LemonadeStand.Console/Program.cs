using NegativeEddy.LemonadeStand;

namespace NegativeEddy.LemonadeStand.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game(new ConsoleIO());
            game.Init();
            while (game.Step())
            {
            }
        }
    }
}
