using System;

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
        private int I; // I: current player number, 1 to N 
        private int NumberOfPlayers; //N: number of players 
        // WeatherFactor: weather factor; 
        // 1 for good weather,
        // 0>WeatherFactor<1 for poor weather;   
        // also adjusts traffic for things like street crews working 
        private double WeatherFactor;
        private int R2; // R2: set to 2 half the time when street department is working;   indicates that street crew bought all lemonade at lunch 
        private decimal CostPerSignDollars; // S3: cost per advertising sign, in dollars 
        private int S2; // number of players?
        private decimal InitialAssets; // initial cash?
        private double C9;
        private decimal CostPerGlassDollars;
        private int C2;

        private int GlassesSold;
        private decimal N1;
        private decimal Expenses;
        private decimal Profit;

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
            Stands = new Stand[30];

            Day = 0;
            MaxPricePerGlassCents = 10;
            CostPerSignDollars = .15M;
            S2 = 30;
            InitialAssets = 2.00M;
            C9 = 0.5;
            C2 = 1;

            Sub12000_TitlePage();
            for (I = 0; I < NumberOfPlayers; I++)
            {
                Stands[I] = new Stand(InitialAssets);
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
            Sub600_CurrentEvents();
        }

        private void Sub600_CurrentEvents()
        {
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

            for (I = 0; I < NumberOfPlayers; I++)
            {
                Stands[I].RuinedByThunderstorm = false;
                Stands[I].H = 0;
                Print($"LEMONADE STAND {I} ASSETS {Stands[I].Assets:C2}");
                Print();
                if (Stands[I].IsBankrupt)
                {
                    Print("YOU ARE BANKRUPT, NO DECISIONS");
                    Print("FOR YOU TO MAKE.");
                    if (NumberOfPlayers == 1 && Stands[0].Assets < CostPerGlassCents)
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
                        Stands[I].GlassesMade = int.Parse(_io.GetInput());
                        if (Stands[I].GlassesMade < 0 || Stands[I].GlassesMade > 1000)
                        {
                            Print("COME ON, LET'S BE REASONABLE NOW!!!");
                            Print("TRY AGAIN");
                            continue;
                        }

                        if (Stands[I].GlassesMade * CostPerGlassDollars <= Stands[I].Assets)
                        {
                            // user can purchase that amount of lemonade
                            break;
                        }
                        Print($"THINK AGAIN!!!  YOU HAVE ONLY {Stands[I].Assets:C2} ");
                        Print($"IN CASH AND TO MAKE {Stands[I].GlassesMade} GLASSES OF ");
                        Print($"LEMONADE YOU NEED ${Stands[I].GlassesMade * CostPerGlassDollars:C2} IN CASH.");
                    }

                    while (true)
                    {
                        Print();
                        Print($"HOW MANY ADVERTISING SIGNS ({CostPerSignDollars * 100} CENTS");
                        Print($"EACH) DO YOU WANT TO MAKE ");
                        Stands[I].SignsMade = int.Parse(_io.GetInput());
                        if (Stands[I].SignsMade < 0 || Stands[I].SignsMade > 50)
                        {
                            Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                            continue;
                        }

                        if (Stands[I].SignsMade * CostPerSignDollars <= Stands[I].Assets - Stands[I].GlassesMade * CostPerGlassDollars)
                        {
                            break;
                        }

                        Print();
                        decimal tmp = Stands[I].Assets - Stands[I].GlassesMade * CostPerGlassDollars;
                        Print($"THINK AGAIN, YOU HAVE ONLY {tmp:C2}");
                        Print("IN CASH LEFT AFTER MAKING YOUR LEMONADE.");
                    }

                    while (true)
                    {
                        Print();
                        Print("WHAT PRICE (IN CENTS) DO YOU WISH TO");
                        Print("CHARGE FOR LEMONADE ");
                        Stands[I].PricePerGlassCents = int.Parse(_io.GetInput());
                        if (Stands[I].PricePerGlassCents <= 0 || Stands[I].PricePerGlassCents >= 100)
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

                if (R2 == 2)
                {
                    Sub2290_StreetCrewsBoughtEverything_Then1185();
                }
            }

            for (I = 0; I < NumberOfPlayers; I++)
            {
                if (Stands[I].Assets < 0)
                {
                    Stands[I].Assets = 0;
                }

                if (R2 != 2)
                {
                    if (Stands[I].PricePerGlassCents < MaxPricePerGlassCents)
                    {
                        N1 = (MaxPricePerGlassCents - Stands[I].PricePerGlassCents) / MaxPricePerGlassCents * .8M * S2 + S2;
                    }
                    else
                    {
                        N1 = MaxPricePerGlassCents * MaxPricePerGlassCents * S2 / (Stands[I].PricePerGlassCents * Stands[I].PricePerGlassCents);
                    }
                    double W = -Stands[I].SignsMade * C9;
                    double V = 1 - Math.Exp(W) * C2;
                    double tmp = WeatherFactor * ((double)N1 + (double)N1 * V);
                    GlassesSold = Stands[I].RuinedByThunderstorm ? 0 : (int)tmp;
                    if (GlassesSold > Stands[I].GlassesMade)
                    {
                        GlassesSold = Stands[I].GlassesMade;
                    }
                }
                else
                {
                    GlassesSold = Stands[I].GlassesMade;
                }

                decimal Income = GlassesSold * Stands[I].PricePerGlassCents * .01M;
                Expenses = Stands[I].SignsMade * CostPerSignDollars + Stands[I].GlassesMade * CostPerGlassDollars;
                Profit = Income - Expenses;
                Stands[I].Assets = Stands[I].Assets + Profit;

                if (Stands[I].H == 1)
                {
                    Sub2300_Thunderstorm_Then1185();
                    continue;   // to 1185
                }
                Print();
                if (Stands[I].IsBankrupt)
                {
                    Print($"STAND {I}");
                    Print("  BANKRUPT");
                    Sub18000_SpaceToContinue();
                }
                else
                {
                    Sub5000_DailyReport(Income);
                    if (Stands[I].Assets <= CostPerGlassCents / 100)
                    {
                        Print($"STAND {I}");
                        Print("  ...YOU DON'T HAVE ENOUGH MONEY LEFT");
                        Print(" TO STAY IN BUSINESS  YOU'RE BANKRUPT!");
                        Stands[I].IsBankrupt = true;
                        Sub18000_SpaceToContinue();
                        if (NumberOfPlayers == 1 && Stands[0].IsBankrupt)
                        {
                            Sub31111_Exit();
                        }
                    }
                }
            }
            WeatherFactor = 1;
            R2 = 0;
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
                R2 = 2;
            }

            return;
        }

        private void Sub2290_StreetCrewsBoughtEverything_Then1185()
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

            for (int J = 0; J < NumberOfPlayers; J++)
            {
                Stands[J].RuinedByThunderstorm = true;
            }
        }

        private void Sub2410()
        {
            Print("A HEAT WAVE IS PREDICTED FOR TODAY!");
            WeatherFactor = 2;
        }

        private void Sub5000_DailyReport(decimal Income)
        {
            Print($"   DAY {Day} STAND {I}");
            Print();
            Print();

            Print($"  {GlassesSold} GLASSES SOLD");
            Print();

            var tmp = Stands[I].PricePerGlassCents / 100.0M;
            Print($"{tmp:C2} PER GLASS");

            Print($"INCOME {Income:C2}");

            Print();
            Print();
            Print($"  {Stands[I].GlassesMade} GLASSES MADE");
            Print();

            Print($"  {Stands[I].SignsMade} SIGNS MADE\t EXPENSES {Expenses:C2}");
            Print();
            Print();

            Print($"  PROFIT {Profit:C}");
            Print();

            Print($"  ASSETS  {Stands[I].Assets:C}");

            Sub18000_SpaceToContinue();
        }

        private void Sub12000_TitlePage()
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

            do
            {
                Print("HOW MANY PEOPLE WILL BE PLAYING?");
                string NS = _io.GetInput();
                NumberOfPlayers = int.Parse(NS);
            }
            while (NumberOfPlayers < 1 || NumberOfPlayers > 30);
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
