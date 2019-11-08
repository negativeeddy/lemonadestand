﻿using System;
using NegativeEddy.LemonadeStand;

namespace NegativeEddy.LemonadeStand.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();

            game.GetInput = System.Console.ReadLine;
            game.Output += (s, e) => System.Console.Write(e.Text);
            game.Run();
        }
    }
}