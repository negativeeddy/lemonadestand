using System;
using System.Linq;

namespace NegativeEddy.LemonadeStand
{
    public partial class Game
    {
        private readonly IGameIO _io;
        private readonly IRandom _random;

        public Game(IGameIO io, IRandom? random = null)
        {
            _io = io;
            _random = random ?? new NetStandardRandom();
        }

        public bool AddNewLinesToOutput { get; set; } = true;

        protected void Print(string text)
        {
            if (AddNewLinesToOutput)
            {
                _io.Output(text + Environment.NewLine);
            }
            else
            {
                _io.Output(text);
            }
        }

        protected void Print()
        {
            if (AddNewLinesToOutput)
            {
                _io.Output(Environment.NewLine);
            }
            else
            {
                _io.Output("");
            }
        }

        /// <summary>
        /// Day of simulation
        /// </summary>
        public int Day { get; private set; } = 0;

        /// <summary>
        /// The list of stands in play
        /// </summary>
        public Stand[]? Stands { get; private set; }

        /// <summary>
        /// cost per advertising sign, in dollars 
        /// </summary>
        public decimal CostPerSignDollars { get; set; } = 0.15M;

        /// <summary>
        /// cost to make a glass of lemonade, in dollars
        /// </summary>
        public decimal CostPerGlassDollars { get; private set; }

        public void Init(decimal intialAssets = 2.0M)
        {
            TitlePage();
            int numPlayers = GetPlayerCount();
            Stands = new Stand[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                Stands[i] = new Stand(i + 1, intialAssets);
            }
            PrintIntro(intialAssets);
        }


        public bool Step()
        {
            if (Stands == null)
            {
                throw new InvalidOperationException("Game not initialized");
            }

            Day++;
            bool ruinedByThunderstorm = false;

            // 1 for good weather,
            // 0>WeatherFactor<1 for poor weather;   
            // also adjusts traffic for things like street crews working 
            double weatherFactor = 1;

            // indicates that street crew bought all lemonade at lunch 
            // this happens half the time when street department is working   
            bool streetCrewBuysEverything = false;

            int skyChance = _random.Next(10);

            SkyOutlook sky;
            if (Day < 3)
            {
                // first days are always Hot and Dry to help newer 
                // players
                sky = SkyOutlook.HotAndDry;
            }
            else
            {
                sky = GetRandomSky(skyChance);
            }

            UpdateCostPerGlass();

            if (Day > 2)
            {
                (weatherFactor, streetCrewBuysEverything) = RandomEvents(sky);
            }


            foreach (Stand stand in Stands)
            {
                Print($"LEMONADE STAND {stand.Id} ASSETS {stand.Assets:C2}");
                Print();
                if (stand.IsBankrupt)
                {
                    Print("YOU ARE BANKRUPT, NO DECISIONS FOR YOU TO MAKE.");
                    if (Stands.Count() == 1 && Stands.First().Assets < CostPerGlassDollars)
                    {
                        return false;
                    }
                }
                else
                {
                    QueryNumberOfGlassesToMake(stand);

                    QueryNumberOfSignsToMake(stand);

                    QueryPriceToSellGlasses(stand);
                }
            }

            Print();

            if (sky == SkyOutlook.Cloudy && _random.Next(100) < 25)
            {
                // thunderstorm happened
                sky = SkyOutlook.Thunderstorms;
                ruinedByThunderstorm = true;
                Print("WEATHER REPORT:  A SEVERE THUNDERSTORM HIT LEMONSVILLE EARLIER TODAY, JUST AS THE LEMONADE STANDS WERE BEING SET UP. UNFORTUNATELY, EVERYTHING WAS RUINED!!");
            }
            else
            {
                Print("$$ LEMONSVILLE DAILY FINANCIAL REPORT $$");
                Print();

                if (streetCrewBuysEverything)
                {
                    Print("THE STREET CREWS BOUGHT ALL YOUR LEMONADE AT LUNCHTIME!!");
                    Print();
                }
            }

            foreach (Stand stand in Stands)
            {
                if (stand.Assets < 0)
                {
                    stand.Assets = 0;
                }

                int glassesSold = CalculateGlassesSold(stand, weatherFactor, streetCrewBuysEverything, ruinedByThunderstorm);

                decimal income = glassesSold * stand.PricePerGlassCents * .01M;
                decimal expenses = stand.SignsMade * CostPerSignDollars + stand.GlassesMade * CostPerGlassDollars;
                decimal profit = income - expenses;
                stand.Assets += profit;

                if (stand.IsBankrupt)
                {
                    Print($"STAND {stand.Id} BANKRUPT");
                }
                else
                {
                    PrintDailyReport(new DailyResult
                    {
                        Day = Day,
                        Stand = stand,
                        Income = income,
                        Expenses = expenses,
                        Profit = profit,
                        GlassesSold = glassesSold,
                    });

                    if (stand.Assets <= CostPerGlassDollars)
                    {
                        Print($"STAND {stand.Id}\n  ...YOU DON'T HAVE ENOUGH MONEY LEFT TO STAY IN BUSINESS YOU'RE BANKRUPT!");
                        stand.IsBankrupt = true;
                        if (Stands.Length == 1 && Stands[0].IsBankrupt)
                        {
                            return false;
                        }
                    }
                }
            }


            return true;
        }

        private static SkyOutlook GetRandomSky(int skyChance)
        {
            SkyOutlook sky;
            if (skyChance < 6)
            {
                sky = SkyOutlook.Sunny;
            }
            else if (skyChance < 8)
            {
                sky = SkyOutlook.Cloudy;
            }
            else
            {
                sky = SkyOutlook.HotAndDry;
            }

            return sky;
        }

        private int CalculateGlassesSold(Stand stand, double weatherFactor, bool streetCrewBuysEverything, bool ruinedByThunderstorm)
        {
            int glassesSold;
            if (streetCrewBuysEverything)
            {
                glassesSold = stand.GlassesMade;
            }
            else if (ruinedByThunderstorm)
            {
                return 0;
            }
            else
            {
                const int MaxPricePerGlassCents = 10;
                const decimal S2 = 30;
                decimal N1;

                if (stand.PricePerGlassCents < MaxPricePerGlassCents)
                {
                    N1 = (MaxPricePerGlassCents - stand.PricePerGlassCents) / MaxPricePerGlassCents * .8M * S2 + S2;
                }
                else
                {
                    N1 = MaxPricePerGlassCents * MaxPricePerGlassCents * S2 / (stand.PricePerGlassCents * stand.PricePerGlassCents);
                }
                double W = -stand.SignsMade * 0.5;
                double V = 1 - Math.Exp(W);
                double tmp = weatherFactor * ((double)N1 + (double)N1 * V);
                glassesSold = (int)tmp;

                if (glassesSold > stand.GlassesMade)
                {
                    // can't sell more glasses than we made
                    glassesSold = stand.GlassesMade;
                }
            }

            return glassesSold;
        }

        private void QueryPriceToSellGlasses(Stand stand)
        {
            while (true)
            {
                Print();
                Print("WHAT PRICE (IN CENTS) DO YOU WISH TO CHARGE FOR LEMONADE ");
                stand.PricePerGlassCents = int.Parse(_io.GetInput());
                if (stand.PricePerGlassCents > 0 && stand.PricePerGlassCents < 100)
                {
                    return;
                }
                Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
            }
        }

        private void QueryNumberOfSignsToMake(Stand stand)
        {
            while (true)
            {
                Print();
                Print($"HOW MANY ADVERTISING SIGNS ({CostPerSignDollars * 100} CENTS EACH) DO YOU WANT TO MAKE ");
                stand.SignsMade = int.Parse(_io.GetInput());
                if (stand.SignsMade < 0 || stand.SignsMade > 50)
                {
                    Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                    continue;
                }

                if (stand.SignsMade * CostPerSignDollars <= stand.Assets - stand.GlassesMade * CostPerGlassDollars)
                {
                    // user has enough money to make signs and glasses
                    return;
                }

                Print();
                decimal tmp = stand.Assets - stand.GlassesMade * CostPerGlassDollars;
                Print($"THINK AGAIN, YOU HAVE ONLY {tmp:C2} IN CASH LEFT AFTER MAKING YOUR LEMONADE.");
            }
        }

        private void QueryNumberOfGlassesToMake(Stand stand)
        {
            while (true)
            {
                Print("HOW MANY GLASSES OF LEMONADE DO YOU WISH TO MAKE ");
                stand.GlassesMade = int.Parse(_io.GetInput());
                if (stand.GlassesMade < 0 || stand.GlassesMade > 1000)
                {
                    Print("COME ON, LET'S BE REASONABLE NOW!!!\nTRY AGAIN");
                    continue;
                }

                if (stand.GlassesMade * CostPerGlassDollars <= stand.Assets)
                {
                    // user can purchase that amount of lemonade
                    return;
                }
                Print($"THINK AGAIN!!!  YOU HAVE ONLY {stand.Assets:C2} IN CASH AND TO MAKE {stand.GlassesMade} GLASSES OF LEMONADE YOU NEED ${stand.GlassesMade * CostPerGlassDollars:C2} IN CASH.");
            }
        }

        private void UpdateCostPerGlass()
        {
            if (Day < 3)
            {
                CostPerGlassDollars = 0.02M;
            }
            else if (Day < 7)
            {
                CostPerGlassDollars = 0.04M;
            }
            else
            {
                CostPerGlassDollars = 0.05M;
            }

            Print($"ON DAY {Day}, THE COST OF LEMONADE IS {CostPerGlassDollars:C2}");

            if (Day == 3)
            {
                Print("(YOUR MOTHER QUIT GIVING YOU FREE SUGAR)");
            }
            else if (Day == 7)
            {
                Print("(THE PRICE OF LEMONADE MIX JUST WENT UP)");
            }
            Print();
        }

        private (double weatherFactor, bool streetCrewBuysEverything) RandomEvents(SkyOutlook sky)
        {
            double weatherFactor = 1;
            bool streetCrewBuysEverything = false;

            switch (sky)
            {
                case SkyOutlook.HotAndDry:
                    Print("A HEAT WAVE IS PREDICTED FOR TODAY!");
                    weatherFactor = 2;
                    break;
                case SkyOutlook.Cloudy:
                    if (_random.Next(100) > 25)
                    {
                        int chanceOfRain = 30 + _random.Next(5) * 10;
                        Print($"THERE IS A {chanceOfRain}% CHANCE OF LIGHT RAIN, AND THE WEATHER IS COOLER TODAY.");
                        weatherFactor = 1 - chanceOfRain / 100.0d;
                    }
                    else
                    {
                        Print("THE STREET DEPARTMENT IS WORKING TODAY. THERE WILL BE NO TRAFFIC ON YOUR STREET.");

                        if (_random.Next(100) < 50)
                        {
                            weatherFactor = 0.1;
                        }
                        else
                        {
                            // 50% of the time the street crew buys all the lemonade
                            streetCrewBuysEverything = true;
                        }
                    }
                    break;
            }
            return (weatherFactor, streetCrewBuysEverything);
        }

        private struct DailyResult
        {
            public int Day;
            public Stand Stand;
            public decimal Income;
            public decimal Profit;
            public decimal Expenses;
            public int GlassesSold;
        }

        private void PrintDailyReport(DailyResult result)
        {
            Print($"  DAY {result.Day} STAND {result.Stand.Id}");
            Print($"  {result.GlassesSold} GLASSES SOLD");

            var tmp = result.Stand.PricePerGlassCents / 100.0M;
            Print($"  {tmp:C2} PER GLASS");
            Print($"  INCOME {result.Income:C2}");
            Print($"  {result.Stand.GlassesMade} GLASSES MADE");
            Print($"  {result.Stand.SignsMade} SIGNS MADE\t EXPENSES {result.Expenses:C2}");
            Print($"  PROFIT {result.Profit:C}");
            Print($"  ASSETS {result.Stand.Assets:C}");
        }

        private void TitlePage()
        {
            Print("HI! WELCOME TO LEMONSVILLE, CALIFORNIA!");
            Print("IN THIS SMALL TOWN, YOU ARE IN CHARGE OF RUNNING YOUR OWN LEMONADE STAND. YOU CAN COMPETE WITH AS MANY OTHER PEOPLE AS YOU WISH, BUT HOW MUCH PROFIT YOU MAKE IS UP TO YOU (THE OTHER STANDS' SALES WILL NOT AFFECT YOUR BUSINESS IN ANY WAY). IF YOU MAKE THE MOST MONEY, YOU'RE THE WINNER!!");
        }

        private int GetPlayerCount()
        {
            int playerCount = -1;

            while (playerCount < 1 || playerCount > 30)
            {
                Print("HOW MANY PEOPLE WILL BE PLAYING?");
                string NS = _io.GetInput();
                int.TryParse(NS, out playerCount);
            }

            return playerCount;
        }

        private void PrintIntro(decimal initialAssets)
        {
            Print("TO MANAGE YOUR LEMONADE STAND, YOU WILL NEED TO MAKE THESE DECISIONS EVERY DAY: ");
            Print();
            Print("1. HOW MANY GLASSES OF LEMONADE TO MAKE (ONLY ONE BATCH IS MADE EACH MORNING)");
            Print("2. HOW MANY ADVERTISING SIGNS TO MAKE (THE SIGNS COST FIFTEEN CENTS EACH)  ");
            Print("3. WHAT PRICE TO CHARGE FOR EACH GLASS  ");
            Print();
            Print($"YOU WILL BEGIN WITH {initialAssets:C2} CASH (ASSETS). BECAUSE YOUR MOTHER GAVE YOU SOME SUGAR, YOUR COST TO MAKE LEMONADE IS TWO CENTS A GLASS (THIS MAY CHANGE IN THE FUTURE).");
            Print();

            Print("YOUR EXPENSES ARE THE SUM OF THE COST OF THE LEMONADE AND THE COST OF THE SIGNS.");
            Print();
            Print("YOUR PROFITS ARE THE DIFFERENCE BETWEEN THE INCOME FROM SALES AND YOUR EXPENSES.");
            Print();
            Print("THE NUMBER OF GLASSES YOU SELL EACH DAY DEPENDS ON THE PRICE YOU CHARGE, AND ON THE NUMBER OF ADVERTISING SIGNS YOU USE.");
            Print();
            Print("KEEP TRACK OF YOUR ASSETS, BECAUSE YOU CAN'T SPEND MORE MONEY THAN YOU HAVE!   ");
            Print();
        }
    }
}
