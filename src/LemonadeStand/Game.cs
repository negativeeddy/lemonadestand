using System;
using System.Linq;

namespace NegativeEddy.LemonadeStand
{
    public partial class Game
    {
        private readonly IGameIO _io;
        private readonly IRandom _random;

        public Game(IGameIO io, IRandom random)
        {
            _io = io;
            _random = random;
        }

        protected void Print(string text)
        {
            _io.Output(text + Environment.NewLine);
        }

        protected void Print()
        {
            _io.Output(Environment.NewLine);
        }

        /// <summary>
        /// Day of simulation
        /// </summary>
        public int Day { get; set; }

        public Stand[] Stands { get; set; }

        private int CostPerGlassCents; // C: cost of lemonade per glass, in cents 
        private int MaxPricePerGlassCents;
        // WeatherFactor: weather factor; 
        // 1 for good weather,
        // 0>WeatherFactor<1 for poor weather;   
        // also adjusts traffic for things like street crews working 
        private double WeatherFactor;
        private bool StreetCrewBuysAll; // R2: set to 2 half the time when street department is working;   indicates that street crew bought all lemonade at lunch 
        private decimal CostPerSignDollars; // S3: cost per advertising sign, in dollars 
        private int S2; // number of players?
        private decimal InitialAssets; // initial cash?

        private decimal CostPerGlassDollars;
        private int C2;

        /// <summary>
        /// sky color (2=sunny, 5=thunderstorms, 7=hot & dry, 10=cloudy).  // TODO: make this an enum?
        /// originally SC: 
        /// </summary>
        private int SkyColor;

        public void Init()
        {
            L135_Initialize();
        }

        private void L135_Initialize()
        {

            Day = 0;
            MaxPricePerGlassCents = 10;
            CostPerSignDollars = .15M;
            S2 = 30;
            InitialAssets = 2.00M;
            C2 = 1;

            TitlePage();
            int numPlayers = GetPlayerCount();
            Stands = new Stand[numPlayers];
            for (int i = 0; i < numPlayers; i++)
            {
                Stands[i] = new Stand(InitialAssets);
            }
            Sub13000_NewBusiness();
        }

        private int J_ChanceOfRain;

        public bool Step()
        {
            SkyColor = _random.Next(10);
            if (SkyColor < 6)
            {
                SkyColor = 2;
            }
            else if (SkyColor < 8)
            {
                SkyColor = 10;
            }
            else
            {
                SkyColor = 7;
            }

            if (Day < 3)
            {
                SkyColor = 2;
            }

            Sub500_StartOfNewDay();

            return true;
        }

        private void Sub500_StartOfNewDay()
        {
            Day++;
            Print($"ON DAY {Day}, THE COST OF LEMONADE IS ");
            if (Day < 3)
            {
                CostPerGlassCents = 2;
            }
            else if (Day < 7)
            {
                CostPerGlassCents = 4;
            }
            else
            {
                CostPerGlassCents = 5;
            }
            Print($"$.0{CostPerGlassCents}");
            Print();
            CostPerGlassDollars = CostPerGlassCents * .01M;
            WeatherFactor = 1;

            if (Day == 3)
            {
                Print("(YOUR MOTHER QUIT GIVING YOU FREE SUGAR)");
            }
            else if (Day == 7)
            {
                Print("(THE PRICE OF LEMONADE MIX JUST WENT UP)");
            }

            if (Day > 2)
            {
                Sub2000_RandomEvents();
            }

            int i = 0;
            foreach (Stand stand in Stands)
            {
                i++;
                stand.RuinedByThunderstorm = false;
                stand.H = 0;
                Print($"LEMONADE STAND {i} ASSETS {stand.Assets:C2}");
                Print();
                if (stand.IsBankrupt)
                {
                    Print("YOU ARE BANKRUPT, NO DECISIONS");
                    Print("FOR YOU TO MAKE.");
                    if (Stands.Count() == 1 && Stands.First().Assets < CostPerGlassCents)
                    {
                        Sub31111_Exit();
                    }
                }
                else
                {
                    while (true)
                    {
                        Print("HOW MANY GLASSES OF LEMONADE DO YOU");
                        Print("WISH TO MAKE ");
                        stand.GlassesMade = int.Parse(_io.GetInput());
                        if (stand.GlassesMade < 0 || stand.GlassesMade > 1000)
                        {
                            Print("COME ON, LET'S BE REASONABLE NOW!!!");
                            Print("TRY AGAIN");
                            continue;
                        }

                        if (stand.GlassesMade * CostPerGlassDollars <= stand.Assets)
                        {
                            // user can purchase that amount of lemonade
                            break;
                        }
                        Print($"THINK AGAIN!!!  YOU HAVE ONLY {stand.Assets:C2} ");
                        Print($"IN CASH AND TO MAKE {stand.GlassesMade} GLASSES OF ");
                        Print($"LEMONADE YOU NEED ${stand.GlassesMade * CostPerGlassDollars:C2} IN CASH.");
                    }

                    while (true)
                    {
                        Print();
                        Print($"HOW MANY ADVERTISING SIGNS ({CostPerSignDollars * 100} CENTS");
                        Print($"EACH) DO YOU WANT TO MAKE ");
                        stand.SignsMade = int.Parse(_io.GetInput());
                        if (stand.SignsMade < 0 || stand.SignsMade > 50)
                        {
                            Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                            continue;
                        }

                        if (stand.SignsMade * CostPerSignDollars <= stand.Assets - stand.GlassesMade * CostPerGlassDollars)
                        {
                            break;
                        }

                        Print();
                        decimal tmp = stand.Assets - stand.GlassesMade * CostPerGlassDollars;
                        Print($"THINK AGAIN, YOU HAVE ONLY {tmp:C2}");
                        Print("IN CASH LEFT AFTER MAKING YOUR LEMONADE.");
                    }

                    while (true)
                    {
                        Print();
                        Print("WHAT PRICE (IN CENTS) DO YOU WISH TO");
                        Print("CHARGE FOR LEMONADE ");
                        stand.PricePerGlassCents = int.Parse(_io.GetInput());
                        if (stand.PricePerGlassCents <= 0 || stand.PricePerGlassCents >= 100)
                        {
                            Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                            continue;
                        }
                        break;
                    }
                }
            }

            Print();
            if (SkyColor == 10 && _random.Next(100) < 25)
            {
                Sub2300_Thunderstorm_Then1185();
            }
            else
            {
                Print("$$ LEMONSVILLE DAILY FINANCIAL REPORT $$");
                Print();

                if (StreetCrewBuysAll)
                {
                    Print_StreetCrewsBoughtEverything();
                }
            }

            i = 0;
            foreach (Stand stand in Stands)
            {
                i++;
                if (stand.Assets < 0)
                {
                    stand.Assets = 0;
                }

                int GlassesSold;

                if (!StreetCrewBuysAll)
                {
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
                    double V = 1 - Math.Exp(W) * C2;
                    double tmp = WeatherFactor * ((double)N1 + (double)N1 * V);
                    GlassesSold = stand.RuinedByThunderstorm ? 0 : (int)tmp;
                    if (GlassesSold > stand.GlassesMade)
                    {
                        GlassesSold = stand.GlassesMade;
                    }
                }
                else
                {
                    GlassesSold = stand.GlassesMade;
                }

                decimal Income = GlassesSold * stand.PricePerGlassCents * .01M;
                decimal Expenses = stand.SignsMade * CostPerSignDollars + stand.GlassesMade * CostPerGlassDollars;
                decimal Profit = Income - Expenses;
                stand.Assets = stand.Assets + Profit;

                if (stand.H == 1)
                {
                    Sub2300_Thunderstorm_Then1185();
                    continue;   // to 1185
                }
                Print();
                if (stand.IsBankrupt)
                {
                    Print($"STAND {i}");
                    Print("  BANKRUPT");
                    Sub18000_SpaceToContinue();
                }
                else
                {
                    Sub5000_DailyReport(new DailyResult
                    {
                        Day = Day,
                        Stand = stand,
                        StandNumber = i,
                        Income = Income,
                        Expenses = Expenses,
                        Profit = Profit,
                        GlassesSold = GlassesSold,
                    });

                    Sub18000_SpaceToContinue();

                    if (stand.Assets <= CostPerGlassCents / 100)
                    {
                        Print($"STAND {i}");
                        Print("  ...YOU DON'T HAVE ENOUGH MONEY LEFT");
                        Print(" TO STAY IN BUSINESS  YOU'RE BANKRUPT!");
                        stand.IsBankrupt = true;
                        Sub18000_SpaceToContinue();
                        if (Stands.Length == 1 && Stands[0].IsBankrupt)
                        {
                            Sub31111_Exit();
                        }
                    }
                }
            }
            WeatherFactor = 1;
            StreetCrewBuysAll = false;
        }

        private void Sub2000_RandomEvents()
        {
            if (SkyColor == 7)
            {
                Sub2410();
                return;
            }

            if (SkyColor != 10)
            {
                return;
            }

            if (_random.Next(100) > 25)
            {
                J_ChanceOfRain = 30 + _random.Next(5) * 10;
                Print($"THERE IS A {J_ChanceOfRain}% CHANCE OF LIGHT RAIN,");
                Print("AND THE WEATHER IS COOLER TODAY.");
                WeatherFactor = 1 - J_ChanceOfRain / 100.0d;
                return;
            }

            Print("THE STREET DEPARTMENT IS WORKING TODAY.");
            Print("THERE WILL BE NO TRAFFIC ON YOUR STREET.");

            if (_random.Next(100) < 50)
            {
                WeatherFactor = 0.1;
            }
            else
            {
                StreetCrewBuysAll = true;
            }

            return;
        }

        private void Print_StreetCrewsBoughtEverything()
        {
            Print("THE STREET CREWS BOUGHT ALL YOUR");
            Print("LEMONADE AT LUNCHTIME!!");
        }

        private void Sub2300_Thunderstorm_Then1185()
        {
            SkyColor = 5;
            Print("WEATHER REPORT:  A SEVERE THUNDERSTORM");
            Print("HIT LEMONSVILLE EARLIER TODAY, JUST AS");
            Print("THE LEMONADE STANDS WERE BEING SET UP.");
            Print("UNFORTUNATELY, EVERYTHING WAS RUINED!!");

            foreach (Stand stand in Stands)
            {
                stand.RuinedByThunderstorm = true;
            }
        }

        private void Sub2410()
        {
            Print("A HEAT WAVE IS PREDICTED FOR TODAY!");
            WeatherFactor = 2;
        }

        private struct DailyResult
        {
            public int Day;
            public Stand Stand;
            public int StandNumber;
            public decimal Income;
            public decimal Profit;
            public decimal Expenses;
            public int GlassesSold;
        }

        private void Sub5000_DailyReport(DailyResult result)
        {
            Print($"   DAY {result.Day} STAND {result.StandNumber}");
            Print();
            Print();

            Print($"  {result.GlassesSold} GLASSES SOLD");
            Print();

            var tmp = result.Stand.PricePerGlassCents / 100.0M;
            Print($"{tmp:C2} PER GLASS");

            Print($"INCOME {result.Income:C2}");

            Print();
            Print();
            Print($"  {result.Stand.GlassesMade} GLASSES MADE");
            Print();

            Print($"  {result.Stand.SignsMade} SIGNS MADE\t EXPENSES {result.Expenses:C2}");
            Print();
            Print();

            Print($"  PROFIT {result.Profit:C}");
            Print();

            Print($"  ASSETS  {result.Stand.Assets:C}");

        }

        private void TitlePage()
        {
            Print("HI!  WELCOME TO LEMONSVILLE, CALIFORNIA!");
            Print();
            Print("IN THIS SMALL TOWN, YOU ARE IN CHARGE OF");
            Print("RUNNING YOUR OWN LEMONADE STAND. YOU CAN");
            Print("COMPETE WITH AS MANY OTHER PEOPLE AS YOU");
            Print("WISH, BUT HOW MUCH PROFIT YOU MAKE IS UP");
            Print("TO YOU (THE OTHER STANDS' SALES WILL NOT");
            Print("AFFECT YOUR BUSINESS IN ANY WAY). IF YOU");
            Print("MAKE THE MOST MONEY, YOU'RE THE WINNER!!");
        }

        private int GetPlayerCount()
        {
            int playerCount = -1;

            do
            {
                Print("HOW MANY PEOPLE WILL BE PLAYING?");
                string NS = _io.GetInput();
                playerCount = int.Parse(NS);
            }
            while (playerCount < 1 || playerCount > 30);

            return playerCount;
        }

        private void Sub13000_NewBusiness()
        {
            Print("TO MANAGE YOUR LEMONADE STAND, YOU WILL ");
            Print("NEED TO MAKE THESE DECISIONS EVERY DAY: ");
            Print();
            Print("1. HOW MANY GLASSES OF LEMONADE TO MAKE    (ONLY ONE BATCH IS MADE EACH MORNING)");
            Print("2. HOW MANY ADVERTISING SIGNS TO MAKE      (THE SIGNS COST FIFTEEN CENTS EACH)  ");
            Print("3. WHAT PRICE TO CHARGE FOR EACH GLASS  ");
            Print();
            Print("YOU WILL BEGIN WITH $2.00 CASH (ASSETS).");
            Print("BECAUSE YOUR MOTHER GAVE YOU SOME SUGAR,");
            Print("YOUR COST TO MAKE LEMONADE IS TWO CENTS ");
            Print("A GLASS (THIS MAY CHANGE IN THE FUTURE).");
            Print();
            Sub18000_SpaceToContinue();

            Print("YOUR EXPENSES ARE THE SUM OF THE COST OF");
            Print("THE LEMONADE AND THE COST OF THE SIGNS. ");
            Print();
            Print("YOUR PROFITS ARE THE DIFFERENCE BETWEEN ");
            Print("THE INCOME FROM SALES AND YOUR EXPENSES.");
            Print();
            Print("THE NUMBER OF GLASSES YOU SELL EACH DAY ");
            Print("DEPENDS ON THE PRICE YOU CHARGE, AND ON ");
            Print("THE NUMBER OF ADVERTISING SIGNS YOU USE.");
            Print();
            Print("KEEP TRACK OF YOUR ASSETS, BECAUSE YOU  ");
            Print("CAN'T SPEND MORE MONEY THAN YOU HAVE!   ");
            Print();

            Sub18000_SpaceToContinue();
        }


        void Sub18000_SpaceToContinue()
        {
            Print(" PRESS ENTER TO CONTINUE, Q TO END...");
            string INS = _io.GetInput();
            if (INS == "Q")
            {
                Sub31111_Exit();
            }
        }

        private void Sub31111_Exit()
        {
            throw new GameOverException("Exiting");
        }
    }

    /// <summary>
    /// Temporary solution to the fact that the original source just quit the process
    /// </summary>
    public class GameOverException : Exception
    {
        public GameOverException(string message) : base(message)
        {
        }
    }
}
