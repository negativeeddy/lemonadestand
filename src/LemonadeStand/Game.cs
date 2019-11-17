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
        /// Weather Factor
        /// 1 for good weather,
        /// 0>WeatherFactor<1 for poor weather;   
        /// also adjusts traffic for things like street crews working 
        /// </summary>
        private double WeatherFactor;

        /// <summary>
        /// indicates that street crew bought all lemonade at lunch 
        /// this happens half the time when street department is working   
        /// </summary>
        private bool StreetCrewBuysEverything;

        /// <summary>
        /// cost per advertising sign, in dollars 
        /// </summary>
        public decimal CostPerSignDollars { get; set; } = 0.15M;

        /// <summary>
        /// Initial cash assets in dollars
        /// </summary>
        public decimal InitialAssets { get; private set; } = 2.00M;

        /// <summary>
        /// cost to make a glass of lemonade, in dollars
        /// </summary>
        public decimal CostPerGlassDollars { get; private set; }

        /// <summary>
        /// if everything is ruined by thunderstorm
        /// originally G(i) : 0 = ruined, 1 = not ruined
        /// </summary>
        private bool RuinedByThunderstorm { get; set; }


        /// <summary>
        /// sky color (2=sunny, 5=thunderstorms, 7=hot & dry, 10=cloudy). 
        /// TODO: make this an enum?
        /// originally SC: 
        /// </summary>
        private SkyColorType SkyColor;

        public void Init(decimal intialAssets = 2.0M)
        {
            InitialAssets = intialAssets;

            TitlePage();
            int numPlayers = GetPlayerCount();
            Stands = new Stand[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                Stands[i] = new Stand(i + 1, InitialAssets);
            }
            PrintIntro();
        }


        public bool Step()
        {
            if (Stands == null)
            {
                throw new InvalidOperationException("Game not initialized");
            }

            Day++;

            int skyChance = _random.Next(10);

            if (skyChance < 6)
            {
                SkyColor = SkyColorType.Clear;
            }
            else if (skyChance < 8)
            {
                SkyColor = SkyColorType.Cloudy;
            }
            else
            {
                SkyColor = SkyColorType.Hot;
            }

            if (Day < 3)
            {
                SkyColor = SkyColorType.Hot;
            }

            UpdateCostPerGlass();

            WeatherFactor = 1;
            if (Day > 2)
            {
                RandomEvents();
            }

            RuinedByThunderstorm = false;

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

            if (SkyColor == SkyColorType.Cloudy && _random.Next(100) < 25)
            {
                // thunderstorm happened
                SkyColor = SkyColorType.Thunderstorms;
                RuinedByThunderstorm = true;
                Print("WEATHER REPORT:  A SEVERE THUNDERSTORM HIT LEMONSVILLE EARLIER TODAY, JUST AS THE LEMONADE STANDS WERE BEING SET UP. UNFORTUNATELY, EVERYTHING WAS RUINED!!");
            }
            else
            {
                Print("$$ LEMONSVILLE DAILY FINANCIAL REPORT $$");
                Print();

                if (StreetCrewBuysEverything)
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

                int GlassesSold = CalculateGlassesSold(stand);

                decimal income = GlassesSold * stand.PricePerGlassCents * .01M;
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
                        GlassesSold = GlassesSold,
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

            WeatherFactor = 1;
            StreetCrewBuysEverything = false;

            return true;
        }

        private int CalculateGlassesSold(Stand stand)
        {
            int GlassesSold;
            if (StreetCrewBuysEverything)
            {
                GlassesSold = stand.GlassesMade;
            }
            else if (RuinedByThunderstorm)
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
                double tmp = WeatherFactor * ((double)N1 + (double)N1 * V);
                GlassesSold = (int)tmp;

                if (GlassesSold > stand.GlassesMade)
                {
                    // can't sell more glasses than we made
                    GlassesSold = stand.GlassesMade;
                }
            }

            return GlassesSold;
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

        private void RandomEvents()
        {
            switch (SkyColor)
            {
                case SkyColorType.Hot:
                    Print("A HEAT WAVE IS PREDICTED FOR TODAY!");
                    WeatherFactor = 2;
                    break;
                case SkyColorType.Cloudy:
                    if (_random.Next(100) > 25)
                    {
                        int chanceOfRain = 30 + _random.Next(5) * 10;
                        Print($"THERE IS A {chanceOfRain}% CHANCE OF LIGHT RAIN, AND THE WEATHER IS COOLER TODAY.");
                        WeatherFactor = 1 - chanceOfRain / 100.0d;
                    }
                    else
                    {
                        Print("THE STREET DEPARTMENT IS WORKING TODAY. THERE WILL BE NO TRAFFIC ON YOUR STREET.");

                        if (_random.Next(100) < 50)
                        {
                            WeatherFactor = 0.1;
                        }
                        else
                        {
                            // 50% of the time the street crew buys all the lemonade
                            StreetCrewBuysEverything = true;
                        }
                    }
                    break;
            }
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

        private void PrintIntro()
        {
            Print("TO MANAGE YOUR LEMONADE STAND, YOU WILL NEED TO MAKE THESE DECISIONS EVERY DAY: ");
            Print();
            Print("1. HOW MANY GLASSES OF LEMONADE TO MAKE (ONLY ONE BATCH IS MADE EACH MORNING)");
            Print("2. HOW MANY ADVERTISING SIGNS TO MAKE (THE SIGNS COST FIFTEEN CENTS EACH)  ");
            Print("3. WHAT PRICE TO CHARGE FOR EACH GLASS  ");
            Print();
            Print($"YOU WILL BEGIN WITH {InitialAssets:C2} CASH (ASSETS). BECAUSE YOUR MOTHER GAVE YOU SOME SUGAR, YOUR COST TO MAKE LEMONADE IS TWO CENTS A GLASS (THIS MAY CHANGE IN THE FUTURE).");
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

    public enum SkyColorType
    {
        Clear = 2,
        Hot = 7,
        Cloudy = 10,
        Thunderstorms = 5,
    }
}
