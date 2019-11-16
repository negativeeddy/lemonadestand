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

        private int C_CostPerGlassCents; // C: cost of lemonade per glass, in cents 
        private int P9_MaxPricePerGlassCents;
        private int I; // I: current player number, 1 to N 
        private string AS;
        private int N_NumberOfPlayers; //N: number of players 
        // R1: weather factor; 
        // 1 for good weather,
        // 0>R<1 for poor weather;   
        // also adjusts traffic for things like street crews working 
        private double R1_WeatherFactor;
        private int R2; // R2: set to 2 half the time when street department is working;   indicates that street crew bought all lemonade at lunch 
        private int R3; // R3: always equal to 0; not used 
        private decimal S3_CostPerSign; // S3: cost per advertising sign, in dollars 
        private int S2; // number of players?
        private decimal InitialAssets; // initial cash?
        private double C9;
        private decimal C1_CostPerGlassDollars;
        private int C2;
        private int SC_SkyColor; // SC: sky color (2=sunny, 5=thunderstorms, 7=hot & dry, 10=cloudy). 
        private int X1; // X1: set to 1 when it's cloudy; not sure what the intent was, but has   no actual effect since line 2100 is unreachable. 
        private int X2; // X2: set to 1 when the street crew has worked and was not thirsty;   apparently intended to prevent street crews from coming again,   but doesn't actually work since line 2200 is unreachable.

        public void Init()
        {
            // 1  LOMEM: 14080
            // 5  GOSUB 10000
            Sub10000();
            // : GOSUB 11000
            Sub11700_Music();
            // : GOSUB 16000

            // : GOTO 135
            L135_Initialize();
            // 10  REM   <<< LEMONADE STAND >>>
            // 15  REM
            // 20  REM  FROM AN ORIGINAL PROGRAM
            // 30  REM   BY BOB JAMISON, OF THE
            // 40  REM   MINNESOTA  EDUCATIONAL
            // 50  REM    COMPUTING CONSORTIUM
            // 60  REM          *  *  *
            // 70  REM   MODIFIED FOR THE APPLE
            // 80  REM       FEBRUARY, 1979
            // 90  REM     BY CHARLIE KELLNER
        }

        private void L135_Initialize()
        {
            // 135  DIM A(30),L(30),H(30),B(30),S(30),P(30),G(30)
            // 150 P9 = 10
            // 160 S3 = .15
            // 170 S2 = 30
            // 175 A2 = 2.00
            // 194	C9 = .5
            // 195	C2 = 1
            Stands = new Stand[30];

            Day = 0;
            P9_MaxPricePerGlassCents = 10;
            S3_CostPerSign = .15M;
            S2 = 30;
            InitialAssets = 2.00M;
            C9 = 0.5;
            C2 = 1;

            // 300  REM   START OF GAME
            // 310  GOSUB 12000
            Sub12000_TitlePage();
            // : FOR I = 1 TO N:B(I) = 0:A(I) = A2: NEXT
            for (I = 0; I < N_NumberOfPlayers; I++)
            {
                Stands[I] = new Stand();
                Stands[I].Bankruptcy = 0;
                Stands[I].Assets = InitialAssets;
            }
            // 320  IF A$ = "Y" THEN  GOSUB 13000: GOTO 400
            // 330  GOSUB 14000
            if (AS == "Y")
            {
                Sub13000_NewBusiness();
            }
            else
            {
                Sub14000_ContinueOldGame();
            }

        }

        private int J_ChanceOfRain;
        private int X4;

        public bool Step()
        {
            // 400  REM   WEATHER REPORT
            // 410 SC =  RND (1)
            SC_SkyColor = _random.Next(10);
            // 420  IF SC < .6 THEN SC = 2: GOTO 460
            // 430  IF SC < .8 THEN SC = 10: GOTO 460
            // 440 SC = 7
            if (SC_SkyColor < 6)
            {
                SC_SkyColor = 2;
            }
            else if (SC_SkyColor < 8)
            {
                SC_SkyColor = 10;
            }
            else
            {
                SC_SkyColor = 7;
            }

            // 460  IF D < 3 THEN SC = 2
            if (Day < 3)
            {
                SC_SkyColor = 2;
            }
            // 470  GOSUB 15000
            Sub15000_WeatherDisplay();

            // 490  TEXT : HOME
            Sub500_StartOfNewDay();

            return true;
        }

        private void Sub500_StartOfNewDay()
        {
            // 500  REM   START OF NEW DAY
            // 510 D = D + 1
            Day++;
            // 520  PRINT "ON DAY ";D;", THE COST OF LEMONADE IS ";
            Print($"ON DAY {Day}, THE COST OF LEMONADE IS ");
            // 540 C = 2
            // : IF D > 2 THEN C = 4
            // 550  IF D > 6 THEN C = 5
            if (Day < 3)
            {
                C_CostPerGlassCents = 2;
            }
            else if (Day < 7)
            {
                C_CostPerGlassCents = 4;
            }
            else
            {
                C_CostPerGlassCents = 5;
            }
            // 560  PRINT "$.0";C
            // : PRINT
            Print($"$.0{C_CostPerGlassCents}");
            Print();
            // 570 C1 = C * .01
            C1_CostPerGlassDollars = C_CostPerGlassCents * .01M;
            // 580 R1 = 1
            R1_WeatherFactor = 1;
            Sub600_CurrentEvents();
        }

        private void Sub600_CurrentEvents()
        {
            // 600  REM   CURRENT EVENTS
            // 610  IF D <  > 3 THEN 650
            // 620  PRINT "(YOUR MOTHER QUIT GIVING YOU FREE SUGAR)"
            // 650  IF D <  > 7 THEN 700
            // 660  PRINT "(THE PRICE OF LEMONADE MIX JUST WENT UP)"
            if (Day == 3)
            {
                Print("(YOUR MOTHER QUIT GIVING YOU FREE SUGAR)");
            }
            else if (Day == 7)
            {
                Print("(THE PRICE OF LEMONADE MIX JUST WENT UP)");
            }

            // 700  REM   AFTER 2 DAYS THINGS CAN HAPPEN
            // 710  IF D > 2 THEN 2000
            if (Day > 2)
            {
                Sub2000_RandomEvents();
            }

            // 800  REM   INPUT VALUES
            // 805  PRINT : POKE 34, PEEK (37) 
            // sets unscrollable window bottom

            // 810  FOR I = 1 TO N
            for (I = 0; I < N_NumberOfPlayers; I++)
            {
                // 820 G(I) = 1:H(I) = 0
                Stands[I].RuinedByThunderstorm = false;
                Stands[I].H = 0;
                // 850 STI = A(I)
                // : GOSUB 4000
                // : PRINT "LEMONADE STAND ";I; TAB( 26);"ASSETS ";STI$ 
                // 855  PRINT
                Print($"LEMONADE STAND {I} ASSETS {Stands[I].Assets:C2}");
                Print();
                // 860  IF B(I) = 0 THEN 890
                if (Stands[I].Bankruptcy != 0)
                {
                    // 870  PRINT "YOU ARE BANKRUPT, NO DECISIONS"
                    // 875	PRINT "FOR YOU TO MAKE."
                    Print("YOU ARE BANKRUPT, NO DECISIONS");
                    Print("FOR YOU TO MAKE.");
                    // 876	IF N = 1 AND A(1) < C THEN 31111
                    if (N_NumberOfPlayers == 1 && Stands[0].Assets < C_CostPerGlassCents)
                    {
                        Sub31111_Exit();
                    }
                    // 880  GOTO 1050
                }
                else
                {
                    while (true)
                    {
                        // 890  PRINT "HOW MANY GLASSES OF LEMONADE DO YOU"
                        // 895  PRINT "WISH TO MAKE ";
                        Print("HOW MANY GLASSES OF LEMONADE DO YOU");
                        Print("WISH TO MAKE ");
                        // 900	INPUT L(I)
                        Stands[I].L_GlassesMade = int.Parse(_io.GetInput());
                        // 901	IF L(I) < 0 OR L(I) > 1000 THEN 903
                        // 902	GOTO 906
                        if (Stands[I].L_GlassesMade < 0 || Stands[I].L_GlassesMade > 1000)
                        {
                            // 903	PRINT "COME ON, LET'S BE REASONABLE NOW!!!"
                            // 904	PRINT "TRY AGAIN"
                            Print("COME ON, LET'S BE REASONABLE NOW!!!");
                            Print("TRY AGAIN");
                            // 905	GOTO 890
                            continue;
                        }

                        // 906	IF L(I) <  >  INT (L(I)) THEN 903
                        // dont need this, input is always an integer

                        // 910  IF L(I) * C1 <  = A(I) THEN 950
                        if (Stands[I].L_GlassesMade * C1_CostPerGlassDollars <= Stands[I].Assets)
                        {
                            // user can purchase that amount of lemonade
                            break;
                        }
                        // 920  PRINT "THINK AGAIN!!!  YOU HAVE ONLY ";STI$
                        // 930  PRINT "IN CASH AND TO MAKE ";L(I);" GLASSES OF"
                        // 932  PRINT "LEMONADE YOU NEED $";L(I) * C1;" IN CASH."
                        Print($"THINK AGAIN!!!  YOU HAVE ONLY {Stands[I].Assets:C2} ");
                        Print($"IN CASH AND TO MAKE {Stands[I].L_GlassesMade} GLASSES OF ");
                        Print($"LEMONADE YOU NEED ${Stands[I].L_GlassesMade * C1_CostPerGlassDollars:C2} IN CASH.");
                        // 940  GOTO 890
                    }

                    while (true)
                    {
                        // 950	PRINT
                        // 951	PRINT "HOW MANY ADVERTISING SIGNS (";S3 * 100;" CENTS"
                        // 952	PRINT "EACH) DO YOU WANT TO MAKE ";
                        Print();
                        Print($"HOW MANY ADVERTISING SIGNS ({S3_CostPerSign * 100} CENTS");
                        Print($"EACH) DO YOU WANT TO MAKE ");
                        // 960	INPUT S(I)
                        Stands[I].S_SignsMade = int.Parse(_io.GetInput());
                        // 961	IF S(I) < 0 OR S(I) > 50 THEN 963
                        // 962	GOTO 965
                        if (Stands[I].S_SignsMade < 0 || Stands[I].S_SignsMade > 50)
                        {
                            // 963	PRINT "COME ON, BE REASONABLE!!! TRY AGAIN."
                            Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                            // 964	GOTO 950
                            continue;
                        }

                        // 965	IF S(I) <  >  INT (S(I)) THEN 963
                        // S[I] is always an int, dont need this check

                        // 970  IF S(I) * S3 <  = A(I) - L(I) * C1 THEN 1010
                        if (Stands[I].S_SignsMade * S3_CostPerSign <= Stands[I].Assets - Stands[I].L_GlassesMade * C1_CostPerGlassDollars)
                        {
                            break;
                        }

                        // 975  PRINT
                        Print();
                        // 980 STI = A(I) - L(I) * C1: GOSUB 4000
                        decimal tmp = Stands[I].Assets - Stands[I].L_GlassesMade * C1_CostPerGlassDollars;
                        // 985  PRINT "THINK AGAIN, YOU HAVE ONLY ";STI$
                        // 990  PRINT "IN CASH LEFT AFTER MAKING YOUR LEMONADE."
                        Print($"THINK AGAIN, YOU HAVE ONLY {tmp:C2}");
                        Print("IN CASH LEFT AFTER MAKING YOUR LEMONADE.");
                        // 1000  GOTO 950
                    }

                    while (true)
                    {
                        // 1010  PRINT : PRINT "WHAT PRICE (IN CENTS) DO YOU WISH TO"
                        // 1012  PRINT "CHARGE FOR LEMONADE ";
                        Print();
                        Print("WHAT PRICE (IN CENTS) DO YOU WISH TO");
                        Print("CHARGE FOR LEMONADE ");
                        // 1015  INPUT P(I)
                        Stands[I].P_PricePerGlassCents = int.Parse(_io.GetInput());
                        // 1020	IF P(I) < 0 OR P(I) > 100 THEN 1022
                        // 1021	GOTO 1024
                        if (Stands[I].P_PricePerGlassCents <= 0 || Stands[I].P_PricePerGlassCents >= 100)
                        {
                            // 1022	PRINT "COME ON, BE REASONABLE!!! TRY AGAIN."
                            Print("COME ON, BE REASONABLE!!! TRY AGAIN.");
                            // 1023	GOTO 1010
                            continue;
                        }
                        // 1024	IF P(I) <  >  INT (P(I)) THEN 1022
                        break;
                        // 1025	IF C5 = 1 THEN 1050
                    }
                }
                // 1050  VTAB 23: INPUT "WOULD YOU LIKE TO CHANGE ANYTHING?";A$
                // 1060  IF  LEFT$ (A$,1) = "Y" THEN  HOME :C5 = 1: GOTO 810
                // this is the "do you want to change anything?" logic
                // ignoring this for simplicity of the loops C5 indicates whether the
                // player accepts all the input
                // 1100	HOME
                // 1101	NEXT I
            }

            // 1110 C5 = 0: TEXT : HOME
            // 1120  PRINT : IF SC = 10 AND  RND (1) < .25 THEN 2300
            Print();
            if (SC_SkyColor == 10 && _random.Next(100) < 25)
            {
                Sub2300_Thunderstorm_Then1185();
            }
            else
            {
                // 1130  PRINT "$$ LEMONSVILLE DAILY FINANCIAL REPORT $$"
                // 1135  PRINT
                Print("$$ LEMONSVILLE DAILY FINANCIAL REPORT $$");
                Print();

                // 1140  POKE 768,152: POKE 769,80: CALL 770
                // 1142  POKE 768,128: POKE 769,160: CALL 770
                // 1144  POKE 768,152: POKE 769,40: CALL 770 1146  POKE 768,144: POKE 769,80: CALL 770
                // 1148  POKE 768,128: POKE 769,200: CALL 770
                // 1180  REM   CALCULATE PROFITS
                if (R2 == 2)
                {
                    // 1182	IF R2 = 2 THEN 2290
                    Sub2290_StreetCrewsBoughtEverything_Then1185();
                }
                else if (R3 == 3)
                {
                    // 1183	IF R3 = 3 THEN 2350
                    // unreachable, R3 is always 0
                }
            }

            // 1185	FOR I = 1 TO N
            for (I = 0; I < N_NumberOfPlayers; I++)
            {
                // 1186	IF A(I) < 0 THEN A(I) = 0
                if (Stands[I].Assets < 0)
                {
                    Stands[I].Assets = 0;
                }

                // 1187	IF R2 = 2 THEN 1260
                if (R2 != 2)
                {
                    // 1190  IF P(I) >  = P9 THEN 1220
                    if (Stands[I].P_PricePerGlassCents < P9_MaxPricePerGlassCents)
                    {
                        // 1200 N1 = (P9 - P(I)) / P9 * .8 * S2 + S2
                        N1 = (P9_MaxPricePerGlassCents - Stands[I].P_PricePerGlassCents) / P9_MaxPricePerGlassCents * .8M * S2 + S2;
                        // 1210  GOTO 1230
                    }
                    else
                    {
                        // 1220 N1 = ((P9 ^ 2) * S2 / P(I) ^ 2)
                        N1 = P9_MaxPricePerGlassCents * P9_MaxPricePerGlassCents * S2 / (Stands[I].P_PricePerGlassCents * Stands[I].P_PricePerGlassCents);
                    }
                    // 1230 W =  - S(I) * C9
                    double W = -Stands[I].S_SignsMade * C9;
                    // 1232 V = 1 - ( EXP (W)     * C2)
                    double V = 1 - Math.Exp(W) * C2;
                    // 1234 N2 = R1 * (N1 + (N1 * V))
                    double tmp = R1_WeatherFactor * ((double)N1 + (double)N1 * V);
                    // 1240 N2 =  INT (N2 * G(I))
                    N2_GlassesSold = Stands[I].RuinedByThunderstorm ? 0 : (int)tmp;
                    // 1250  IF N2 <  = L(I) THEN 1270
                    if (N2_GlassesSold > Stands[I].L_GlassesMade)
                    {
                        // repeat line 1260 logic here
                        N2_GlassesSold = Stands[I].L_GlassesMade;
                    }
                }
                else
                {
                    // 1260 N2 = L(I)
                    N2_GlassesSold = Stands[I].L_GlassesMade;
                }

                // 1270 M = N2 * P(I) * .01
                M_Income = N2_GlassesSold * Stands[I].P_PricePerGlassCents * .01M;
                // 1280 E = S(I) * S3 + L(I) * C
                E_Expenses = Stands[I].S_SignsMade * S3_CostPerSign + Stands[I].L_GlassesMade * C1_CostPerGlassDollars;
                // 1290 P1 = M - E
                P1_Profit = M_Income - E_Expenses;
                // 1300 A(I) = A(I) + P1
                Stands[I].Assets = Stands[I].Assets + P1_Profit;

                // 1310  IF H(I) = 1 THEN 2300
                if (Stands[I].H == 1)
                {
                    Sub2300_Thunderstorm_Then1185();
                    continue;   // to 1185
                }
                // 1320	PRINT
                Print();
                // 1321	IF B(I) <  > 1 THEN 1330
                if (Stands[I].Bankruptcy == 1)
                {
                    // 1326	PRINT "STAND ";I;: PRINT "   BANKRUPT": GOSUB 18000
                    Print($"STAND {I}");
                    Print("  BANKRUPT");
                    Sub18000_SpaceToContinue();
                    // 1327	GOTO 1390
                }
                else
                {
                    // 1330  GOSUB 5000
                    Sub5000_DailyReport();
                    // 1350  IF A(I) > C / 100 THEN 1390
                    if (Stands[I].Assets <= C_CostPerGlassCents / 100)
                    {
                        // 1360  PRINT "STAND ";I
                        Print($"STAND {I}");
                        // 1365  HOME : PRINT "  ...YOU DON'T HAVE ENOUGH MONEY LEFT"
                        // 1370  PRINT " TO STAY IN BUSINESS  YOU'RE BANKRUPT!"
                        Print("  ...YOU DON'T HAVE ENOUGH MONEY LEFT");
                        Print(" TO STAY IN BUSINESS  YOU'RE BANKRUPT!");
                        // 1380 B(I) = 1
                        Stands[I].Bankruptcy = 1;
                        // 1382  GOSUB 18000: HOME
                        Sub18000_SpaceToContinue();
                        // 1385  IF N = 1 AND B(1) = 1 THEN 31111
                        if (N_NumberOfPlayers == 1 && Stands[0].Bankruptcy == 1)
                        {
                            Sub31111_Exit();
                        }
                    }
                }
                // 1390  NEXT I
            }
            // 1395 R1 = 1 
            R1_WeatherFactor = 1;
            // 1396 R2 = 0
            R2 = 0;
            // 1400  GOTO 400
        }

        private void Sub2000_RandomEvents()
        {
            // 2000  REM   RANDOM EVENTS
            // 2010  IF SC = 10 THEN 2110
            // 2030  IF SC = 7 THEN 2410
            if (SC_SkyColor == 7)
            {
                Sub2410();
                return;
            }
            if (SC_SkyColor != 10)
            {
                // 2050  GOTO 805
                return;
                // 2100  IF X1 = 1 THEN 805    { unreachable }
            }
            // 2040  IF  RND (1) < .25 THEN 2210
            if (_random.Next(100) > 25)
            {
                // 2110 J = 30 +  INT ( RND (1) * 5) * 10
                J_ChanceOfRain = 30 + _random.Next(5) * 10;
                // 2112  PRINT "THERE IS A ";J;"% CHANCE OF LIGHT RAIN,"
                Print($"THERE IS A {J_ChanceOfRain}% CHANCE OF LIGHT RAIN,");
                // 2115  PRINT "AND THE WEATHER IS COOLER TODAY."
                Print("AND THE WEATHER IS COOLER TODAY.");
                // 2120 R1 = 1 - J / 100
                R1_WeatherFactor = 1 - J_ChanceOfRain / 100.0d;
                // 2130 X1 = 1
                X1 = 1;
                // 2140  GOTO 805
                return;
                // 2200  IF X2 = 1 THEN 805    { unreachable }
            }

            // 2210  PRINT "THE STREET DEPARTMENT IS WORKING TODAY."
            // 2220  PRINT "THERE WILL BE NO TRAFFIC ON YOUR STREET."
            Print("THE STREET DEPARTMENT IS WORKING TODAY.");
            Print("THERE WILL BE NO TRAFFIC ON YOUR STREET.");

            // 2230	IF  RND ( - 1) < .5 THEN 2233   { bug?  rnd(-1) is a constant! }
            // 2231	R2 = 2
            // 2232	GOTO 2250
            // 2233	R1 = .1
            if (_random.Next(100) < 50)
            {
                R1_WeatherFactor = 0.1;
            }
            else
            {
                R2 = 2;
            }

            // 2250 X2 = 1
            X2 = 1;
            // 2260  GOTO 805
            return;

        }

        private void Sub2290_StreetCrewsBoughtEverything_Then1185()
        {
            // 2290  PRINT "THE STREET CREWS BOUGHT ALL YOUR"
            // 2295  PRINT "LEMONADE AT LUNCHTIME!!"
            Print("THE STREET CREWS BOUGHT ALL YOUR");
            Print("LEMONADE AT LUNCHTIME!!");
            // 2297  GOTO 1185
        }

        private int X3;

        private void Sub2300_Thunderstorm_Then1185()
        {
            // 2300  REM   THUNDERSTORM!
            // 2310 X3 = 1:R3 = 0
            X3 = 1;
            R3 = 0;
            // 2320 SC = 5: GOSUB 15000: TEXT : HOME
            SC_SkyColor = 5;
            Sub15000_WeatherDisplay();
            // 2330  PRINT "WEATHER REPORT:  A SEVERE THUNDERSTORM"
            // 2340  PRINT "HIT LEMONSVILLE EARLIER TODAY, JUST AS"
            // 2350  PRINT "THE LEMONADE STANDS WERE BEING SET UP."
            // 2360  PRINT "UNFORTUNATELY, EVERYTHING WAS RUINED!!"
            Print("WEATHER REPORT:  A SEVERE THUNDERSTORM");
            Print("HIT LEMONSVILLE EARLIER TODAY, JUST AS");
            Print("THE LEMONADE STANDS WERE BEING SET UP.");
            Print("UNFORTUNATELY, EVERYTHING WAS RUINED!!");

            // 2370  FOR J = 1 TO N:G(J) = 0: NEXT
            for (int J = 0; J < N_NumberOfPlayers; J++)
            {
                Stands[J].RuinedByThunderstorm = true;
            }
            // 2380  GOTO 1185
        }

        private void Sub2400()
        {
            // unreachable?
            // 2400  IF X4 = 1 THEN 805
        }

        private void Sub2410()
        {
            // 2410 X4 = 1
            X4 = 1;
            // 2430  PRINT "A HEAT WAVE IS PREDICTED FOR TODAY!"
            Print("A HEAT WAVE IS PREDICTED FOR TODAY!");
            // 2440 R1 = 2
            R1_WeatherFactor = 2;
            // 2450  GOTO 805
        }

        // 3000  END

        private int N2_GlassesSold;
        private decimal N1;
        private decimal M_Income;
        private decimal E_Expenses;
        private decimal P1_Profit;

        private void Sub5000_DailyReport()
        {
            // 5000  VTAB 6: POKE 34,5
            // 5002  PRINT "   DAY ";D; TAB( 30);"STAND ";I: PRINT : PRINT
            Print($"   DAY {Day} STAND {I}");
            Print();
            Print();

            // 5010  PRINT "  ";N2; TAB( 7);"GLASSES SOLD": PRINT
            Print($"  {N2_GlassesSold} GLASSES SOLD");
            Print();

            // 5012 STI = P(I) / 100: GOSUB 4000: PRINT STI$; TAB( 7);"PER GLASS";
            var tmp = Stands[I].P_PricePerGlassCents / 100.0M;
            Print($"{tmp:C2} PER GLASS");

            // 5014 STI = M: GOSUB 4000: PRINT  TAB( 27);"INCOME ";STI$
            Print($"INCOME {M_Income:C2}");

            // 5016  PRINT : PRINT : PRINT "  ";L(I); TAB( 7);"GLASSES MADE": PRINT
            Print();
            Print();
            Print($"  {Stands[I].L_GlassesMade} GLASSES MADE");
            Print();

            // 5020 STI = E: GOSUB 4000: PRINT "  ";S(I); TAB( 7);"SIGNS MADE"; TAB( 25);"EXPENSES ";STI $: PRINT : PRINT
            Print($"  {Stands[I].S_SignsMade} SIGNS MADE\t EXPENSES {E_Expenses:C2}");
            Print();
            Print();

            // 5030 STI = P1: GOSUB 4000: PRINT  TAB( 16);"PROFIT  ";STI$: PRINT
            Print($"  PROFIT {P1_Profit:C}");
            Print();

            // 5040 STI = A(I): GOSUB 4000: PRINT  TAB( 16);"ASSETS  ";STI$
            Print($"  ASSETS  {Stands[I].Assets:C}");

            // 5060  GOSUB 18000
            Sub18000_SpaceToContinue();
            // 5070  REM
            // 5090  HOME : RETURN
        }

        private void Sub10000()
        {
            // 10000  REM   INITIALIZE 10010  IN# 0: PR# 0
            // 10100  POKE 770,173: POKE 771,48: POKE 772,192: POKE 773,136: POKE 774,208: POKE 775,5: POKE 776,206: POKE 777,1: POKE 778,3: POKE 779,240: POKE 780,9: POKE 781,202
            // 10110  POKE 782,208: POKE 783,245: POKE 784,174: POKE 785,0: POKE 786,3: POKE 787,76: POKE 788,2: POKE 789,3: POKE 790,96: POKE 791,0: POKE 792,0
            // 10190  RETURN
        }

        private void Sub11000_Introduction()
        {
            // 11000  REM   INTRODUCTION
            // 11100  TEXT 
            // : HOME 
            // : GR 
            // : COLOR= 12
            // : FOR I = 0 TO 39
            // : HLIN 0,39 AT I
            // : NEXT 
            // : VTAB 2
            // 11110  PRINT ";LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL;LLLLL";
            // 11120  PRINT ";LLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL;LLLLL";
            // 11130  PRINT ";LLLL;;;;L;;;;;L;;;;L;;;;L;;;;L;;;;L;;;;";
            // 11140  PRINT ";LLLL;LL;L;L;L;L;LL;L;LL;LLLL;L;LL;L;LL;";
            // 11150  PRINT ";LLLL;;;;L;L;L;L;LL;L;LL;L;;;;L;LL;L;;;;";
            // 11160  PRINT ";LLLL;LLLL;L;L;L;LL;L;LL;L;LL;L;LL;L;LLL";
            // 11170  PRINT ";;;;L;;;;L;LLL;L;;;;L;LL;L;;;;L;;;;L;;;;";
            // 11200  VTAB 11
            // 11210  PRINT "LLLLLLLL;;;;;LL;LLLLLLLLLLLLLLL;LLLLLLLL";
            // 11220  PRINT "LLLLLLLL;LLLLLL;LLLLLLLLLLLLLLL;LLLLLLLL";
            // 11230  PRINT "LLLLLLLL;LLLLL;;;L;;;;L;;;;L;;;;LLLLLLLL";
            // 11240  PRINT "LLLLLLLL;;;;;LL;LLLLL;L;LL;L;LL;LLLLLLLL";
            // 11250  PRINT "LLLLLLLLLLLL;LL;LL;;;;L;LL;L;LL;LLLLLLLL";
            // 11260  PRINT "LLLLLLLLLLLL;LL;LL;LL;L;LL;L;LL;LLLLLLLL";
            // 11270  PRINT "LLLLLLLL;;;;;LL;LL;;;;L;LL;L;;;;LLLLLLLL";
            // 11280  VTAB 23: PRINT "  COPYRIGHT 1979    APPLE COMPUTER INC."
            // 11300  GOSUB 11700
            Sub11700_Music();
            //: FOR I = 1 TO 2000: NEXT

            // 11310  POKE 33,1: POKE 34,9: POKE 35,19
            // 11320  FOR I = 39 TO 7 STEP  - 1: COLOR= 15: VLIN 18,37 AT I: COLOR= 12: IF I < 39 
            // THEN  VLIN 18,36 AT I + 1
            // 11330  COLOR= 15: IF I < 30 THEN  VLIN 18,36 AT I + 10
            // 11340  COLOR= 12: IF I < 29 THEN  VLIN 18,37 AT I + 11: POKE 32,I + 11: VTAB 11: 
            // HTAB I + 12: PRINT : READ A$: PRINT A$;
            // 11350  FOR J = 14 TO I + I: NEXT : NEXT : FOR I = 1 TO 1000: NEXT
            // 11360  COLOR= 13: VLIN 14,36 AT 12: FOR I = 36 TO 20 STEP  - 1: HLIN 8,16 AT I
            // 11370  FOR J = 1 TO 50: NEXT : POKE 768,I * 3 - 12: POKE 769,12: CALL 770: NEXT : 
            // COLOR= 12: VLIN 14,19 AT 12
            // 11400  POKE 32,0: POKE 33,40: POKE 34,20: POKE 35,23: VTAB 22
            // 11410  FOR I = 1 TO 4000: NEXT :I =  FRE (0)
            // 11490  RETURN
        }

        // 11500  REM   DATA
        // 11510  DATA 
        // 96,180,128,60,114,60,128,120,144,60,152,60,128,255,128,60,114,60,85,120,96,60,102 ,60,114,120,102,60,96,255,0,0
        // 11520  DATA ,,,,,,L;LL;,L;LL;,;;LL;;,L;LL;,L;;;;,,,;L;;;;,;L;LL;,;L;LL;,;L;LL;,;;;LL;,,,,,*
        // 11530  DATA  96,16,85,4,128,4,96,4,76,4,128,4,96,16,0,0
        // 11540  DATA 
        // 114,120,144,60,114,255,1,120,128,120,144,60,128,120,114,60,144,120,171,255,228,2 55,0,0
        // 11550  DATA 152,180,152,120,152,60,144,120,152,60,171,120,192,60,152,255,0,0
        // 11560  DATA 
        // 0,160,128,255,152,40,171,80,192,40,228,255,1,40,0,160,192,255,192,40,171,80,152,4 0,128,255,0,0

        private void Sub11700_Music()
        {
            // 11700  REM   MUSIC
            // 11710  READ I,J: IF J = 0 THEN  RETURN

            // 11720  POKE 768,I: POKE 769,J: CALL 770
            // 11730  GOTO 11710
        }

        private void Sub12000_TitlePage()
        {
            // 12000  REM   TITLE PAGE
            // 12100  TEXT : HOME : PRINT  CHR$ (7);
            // 12110  PRINT "HI!  WELCOME TO LEMONSVILLE, CALIFORNIA!": PRINT
            // 12120  PRINT "IN THIS SMALL TOWN, YOU ARE IN CHARGE OF"
            // 12130  PRINT "RUNNING YOUR OWN LEMONADE STAND. YOU CAN"
            // 12140  PRINT "COMPETE WITH AS MANY OTHER PEOPLE AS YOU"
            // 12150  PRINT "WISH, BUT HOW MUCH PROFIT YOU MAKE IS UP"
            // 12160  PRINT "TO YOU (THE OTHER STANDS' SALES WILL NOT"
            // 12170  PRINT "AFFECT YOUR BUSINESS IN ANY WAY). IF YOU"
            // 12180  PRINT "MAKE THE MOST MONEY, YOU'RE THE WINNER!!"
            // : PRINT
            // 12190  PRINT "ARE YOU STARTING A NEW GAME? (YES OR NO)"
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
                Print();
                Print("ARE YOU STARTING A NEW GAME? (YES OR NO)");

                // 12200  VTAB 21: CALL  - 958
                // : INPUT "TYPE YOUR ANSWER AND HIT RETURN ==> ";A$ 
                AS = _io.GetInput();
                // 12210 A$ =  LEFT$ (A$,1)
                if (AS.Length > 0)
                {
                    AS = AS.Substring(0, 1).ToUpper();
                }
                // : IF A$ <  > "Y" AND A$ <  > "N" THEN  PRINT CHR$ (7);: GOTO 12200
            }
            while (AS != "Y" && AS != "N");

            do
            {
                // 12220  VTAB 23: CALL  - 958
                // : INPUT "HOW MANY PEOPLE WILL BE PLAYING ==> ";N$
                Print("HOW MANY PEOPLE WILL BE PLAYING?");
                string NS = _io.GetInput();
                // 12230 N =  VAL (N$)
                N_NumberOfPlayers = int.Parse(NS);
                // : IF N < 1 OR N > 30 THEN  PRINT  CHR$ (7);: GOTO 12220 
            }
            while (N_NumberOfPlayers < 1 || N_NumberOfPlayers > 30);
            // 12240 RETURN
        }

        private void Sub13000_NewBusiness()
        {
            // 13000  REM   NEW BUSINESS
            // 13100  HOME
            // 13110  PRINT "TO MANAGE YOUR LEMONADE STAND, YOU WILL "
            // 13120  PRINT "NEED TO MAKE THESE DECISIONS EVERY DAY: ": PRINT
            // 13130  PRINT "1. HOW MANY GLASSES OF LEMONADE TO MAKE    (ONLY ONE BATCH IS MADE EACH MORNING)"
            // 13140  PRINT "2. HOW MANY ADVERTISING SIGNS TO MAKE      (THE SIGNS COST FIFTEEN CENTS EACH)  "
            // 13150  PRINT "3. WHAT PRICE TO CHARGE FOR EACH GLASS  ": PRINT
            // 13160  PRINT "YOU WILL BEGIN WITH $2.00 CASH (ASSETS)."
            // 13170  PRINT "BECAUSE YOUR MOTHER GAVE YOU SOME SUGAR,"
            // 13180  PRINT "YOUR COST TO MAKE LEMONADE IS TWO CENTS "
            // 13190  PRINT "A GLASS (THIS MAY CHANGE IN THE FUTURE).": PRINT
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
            // 13200  GOSUB 18000
            Sub18000_SpaceToContinue();

            // 13202  HOME
            // 13210  PRINT "YOUR EXPENSES ARE THE SUM OF THE COST OF"
            // 13220  PRINT "THE LEMONADE AND THE COST OF THE SIGNS. ": PRINT
            // 13230  PRINT "YOUR PROFITS ARE THE DIFFERENCE BETWEEN "
            // 13240  PRINT "THE INCOME FROM SALES AND YOUR EXPENSES.": PRINT
            // 13250  PRINT "THE NUMBER OF GLASSES YOU SELL EACH DAY "
            // 13260  PRINT "DEPENDS ON THE PRICE YOU CHARGE, AND ON "
            // 13270  PRINT "THE NUMBER OF ADVERTISING SIGNS YOU USE.": PRINT
            // 13280  PRINT "KEEP TRACK OF YOUR ASSETS, BECAUSE YOU  "
            // 13290  PRINT "CAN'T SPEND MORE MONEY THAN YOU HAVE!   ": PRINT
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

            // 13300  GOSUB 18000
            Sub18000_SpaceToContinue();
            // 13302  HOME : RETURN
        }

        private void Sub14000_ContinueOldGame()
        {
            throw new NotImplementedException();
            // 14000  REM   CONTINUE OLD GAME
            // 14100  CALL  - 936: PRINT  CHR$ (7);:I = 0
            I = 0;
            // 14110  PRINT "HI AGAIN!  WELCOME BACK TO LEMONSVILLE! ": PRINT
            // 14120  PRINT "LET'S CONTINUE YOUR LAST GAME FROM WHERE"
            // 14130  PRINT "YOU LEFT IT LAST TIME.  DO YOU REMEMBER "
            // 14140  PRINT "WHAT DAY NUMBER IT WAS? ";
            Print("HI AGAIN!  WELCOME BACK TO LEMONSVILLE! ");
            Print();
            Print("LET'S CONTINUE YOUR LAST GAME FROM WHERE");
            Print("YOU LEFT IT LAST TIME.  DO YOU REMEMBER ");
            Print("WHAT DAY NUMBER IT WAS? ");

            // 14150  INPUT "";A$:A =  VAL (A$)
            // : PRINT 
            // : IF A <  > 0 THEN 14200
            {
                // 14160 A$ =  LEFT$ (A$,1): IF A$ = "Y" THEN  PRINT "GOOD!  WHAT DAY WAS IT? ";:I = I + 1: GOTO 14150
                // 14170  IF A$ = "N" OR I > 0 THEN 14300
                // 14180  PRINT  CHR$ (7);"YES OR NO? ";:I = I + 1: GOTO 14150
            }
            // 14200  IF A < 1 OR A > 99 OR A <  >  INT (A) THEN 14300
            // 14210 D = A
            // 14300  PRINT "OKAY - WE'LL START WITH DAY NO. ";D + 1: PRINT
            // 14400  FOR I = 1 TO N: PRINT : PRINT
            // 14410  PRINT "PLAYER NO. ";I;", HOW MUCH MONEY (ASSETS)": PRINT
            // 14420  PRINT "DID YOU HAVE? ";
            // 14430  INPUT "";A$:A =  VAL (A$): PRINT
            // 14440  IF A < 2 THEN  PRINT "O.K. - WE'LL START YOU OUT WITH $2.00":A = 2: GOTO 14490
            // 14450  IF A > 40 THEN  PRINT "JUST TO BE FAIR, LET'S MAKE THAT $10.00":A = 10
            // 14490 A(I) =  INT (A * 100 + .5) / 100: NEXT
            // 14500  PRINT : PRINT  CHR$ (7): INPUT "...READY TO BEGIN? ";A$
            // 14510  IF  LEFT$ (A$,1) = "N" THEN 13000
            // 14520  RETURN
        }

        private void Sub15000_WeatherDisplay()
        {
            // 15000  REM   WEATHER DISPLAY
            // 15100  GR : HOME
            // 15110  COLOR= SC: FOR I = 0 TO 25: HLIN 0,39 AT I: NEXT I
            // 15120  COLOR= 12: FOR I = 26 TO 39: HLIN 0,39 AT I: NEXT I 15130  COLOR= 8: FOR I = 24 TO 32: HLIN 15,25 AT I: NEXT I
            // 15150	COLOR= 13: FOR I = 17 TO 23 STEP 2: VLIN 22,23 AT I: NEXT I
            // 15151	IF SC = 2 OR SC = 7 THEN 15160
            // 15152	IF SC = 10 THEN  COLOR= 15
            // 15157	IF SC = 5 THEN  COLOR= 0
            // 15158	HLIN 6,10 AT 2: HLIN 4,14 AT 3: HLIN 7,12 AT 4: HLIN 22,30 AT 4: HLIN 20,36 AT 
            // 5: HLIN 23,33 AT 6::: GOSUB 17000: GOTO 15170
            // 15160  IF SC = 7 THEN  COLOR= 9
            // 15162  HLIN 3,5 AT 1: HLIN 2,6 AT 2: FOR I = 3 TO 6: HLIN 1,7 AT I: NEXT : HLIN 2,6 AT 7: 
            // HLIN 3,5 AT 8
            // 15170  VTAB 22: HTAB 8: PRINT " LEMONSVILLE WEATHER REPORT ": PRINT
            // 15175  VTAB 22: HTAB 8: PRINT " LEMONSVILLE WEATHER REPORT ": PRINT
            // 15180  IF SC = 2 THEN  HTAB 18: PRINT " SUNNY ";
            // 15182  IF SC = 7 THEN  HTAB 15: PRINT " HOT AND DRY ";
            // 15184  IF SC = 10 THEN  HTAB 17: PRINT " CLOUDY ";
            // 15186  IF SC = 5 THEN  HTAB 14: PRINT " THUNDERSTORMS! ";: GOSUB 17000 15200  RESTORE
            // 15210  READ A$: IF A$ <  > "*" THEN 15210
            // 15220  IF SC <  > 2 THEN 15300
            // 15230  READ I,J: IF J = 0 THEN 15500
            // 15240  FOR K = 1 TO J: POKE 768,I: POKE 769,10: CALL 770
            // 15250  FOR L = K TO J: NEXT : NEXT : GOTO 15230
            // 15300  READ I,J: IF J <  > 0 THEN 15300
            // 15310  IF SC = 7 THEN 15400
            // 15320  READ I,J: IF J <  > 0 THEN 15320
            // 15330  IF SC = 10 THEN 15400
            // 15340  READ I,J: IF J <  > 0 THEN 15340
            // 15400  READ I,J: IF J = 0 THEN 15500
            // 15410  IF I = 1 THEN  FOR I = 1 TO J: NEXT : GOTO 15400
            // 15420  POKE 768,I: POKE 769,J: CALL 770
            // 15430  GOTO 15400
            // 15440  IF SC = 5 THEN  GOSUB 17000 15500  IF SC = 5 THEN  GOSUB 17000
            // 15510 I =  FRE (0)
            // : FOR I = 1 TO 2000: NEXT 
            // : RETURN
        }

        void Sub16000()
        {
            // 16000  POKE 13824,76: POKE 13825,108: POKE 13826,54: POKE 13827,165: POKE 13828,48: POKE 13829,41: POKE 13830,15: POKE 13831,141: POKE 13832,160: POKE 13833,54:
            // 16010  POKE 13834,165: POKE 13835,48: POKE 13836,41: POKE 13837,240: POKE 13838,141: POKE 13839,161: POKE 13840,54: POKE 13841,165: POKE 13842,36: POKE 13843,141:
            // 16020  POKE 13844,162: POKE 13845,54: POKE 13846,165: POKE 13847,37: POKE 13848,141: POKE 13849,163: POKE 13850,54: POKE 13851,169: POKE 13852,0: POKE 13853,133:
            // 16030  POKE 13854,36: POKE 13855,133: POKE 13856,37: POKE 13857,72: POKE 13858,32: POKE 13859,193: POKE 13860,251: POKE 13861,133: POKE 13862,40: POKE 13863,164:
            // 16040  POKE 13864,36: POKE 13865,177: POKE 13866,40: POKE 13867,41: POKE 13868,15: POKE 13869,205: POKE 13870,164: POKE 13871,54: POKE 13872,208: POKE 13873,10:
            // 16050  POKE 13874,177: POKE 13875,40: POKE 13876,41: POKE 13877,240: POKE 13878,24: POKE 13879,109: POKE 13880,160: POKE 13881,54: POKE 13882,145: POKE 13883,40:
            // 16060  POKE 13884,177: POKE 13885,40: POKE 13886,41: POKE 13887,240: POKE 13888,205: POKE 13889,165: POKE 13890,54: POKE 13891,208: POKE 13892,10: POKE 13893,177:
            // 16070  POKE 13894,40: POKE 13895,41: POKE 13896,15: POKE 13897,24: POKE 13898,109: POKE 13899,161: POKE 13900,54: POKE 13901,145: POKE 13902,40: POKE 13903,200:
            // 16080  POKE 13904,192: POKE 13905,40: POKE 13906,144: POKE 13907,213: POKE 13908,160: POKE 13909,0: POKE 13910,104: POKE 13911,105: POKE 13912,0: POKE 13913,197:
            // 16090  POKE 13914,34: POKE 13915,144: POKE 13916,196: POKE 13917,32: POKE 13918,193: POKE 13919,251: POKE 13920,133: POKE 13921,40: POKE 13922,173: POKE 13923,162:
            // 16100  POKE 13924,48: POKE 13925,133: POKE 13926,36: POKE 13927,173: POKE 13928,163: POKE 13929,54: POKE 13930,133: POKE 13931,37: POKE 13932,165: POKE 13933,48:
            // 16110  POKE 13934,41: POKE 13935,15: POKE 13936,141: POKE 13937,164: POKE 13938,54: POKE 13939,165: POKE 13940,48: POKE 13941,41: POKE 13942,240: POKE 13943,141:
            // 16120  POKE 13944,165: POKE 13945,54: POKE 13946,96: POKE 13947,0:
            // 16130  RETURN
        }

        private void Sub17000()
        {
            // 17000	IF SC <  > 5 THEN  RETURN
            // 17001	COLOR= 10: VLIN 7,9 AT 29: HLIN 30,31 AT 9: VLIN 9,14 AT 32: HLIN 33,34 AT 14: VLIN 14,25 AT 35
            // 17005  VLIN 5,8 AT 8: PLOT 9,8: VLIN 8,13 AT 10: PLOT 11,13: VLIN 13,17 AT 12
            // 17010  FOR RPT = 1 TO 5
            // 17020  CALL 13824
            // 17030  COLOR= 7: CALL 13827
            // 17040  CALL 13824
            // 17050  COLOR= 9: CALL 13827
            // 17060  NEXT RPT
            // 17070  CALL 13824
            // 17080  COLOR= 5: CALL 13827
            // 17090  RETURN
        }

        void Sub18000_SpaceToContinue()
        {
            // 18000  VTAB 24: PRINT " PRESS SPACE TO CONTINUE, ESC TO END...";
            Print(" PRESS ENTER TO CONTINUE, Q TO END...");
            // 18010  GET IN$: IF IN$ <  > " " AND  ASC (IN$) <  > 27 THEN 18010
            string INS = _io.GetInput();
            // 18020  IF  ASC (IN$) = 27 THEN 31111
            if (INS == "Q")
            {
                Sub31111_Exit();
            }
            // 18030  RETURN
        }

        private void Sub31111_Exit()
        {
            // 31111	FOR I = 1 TO 2000: NEXT : HOME : VTAB 10: PRINT "WOULD YOU LIKE TO PLAY AGAIN ";
            // 31112	GET IN$: IF IN$ = "Y" THEN 150
            // 31113	IF  PEEK (994) +  PEEK (1001) = 192 THEN  CALL 976 31114  TEXT : HOME 
            // : TEXT 
            // : END
            throw new Exception("Exiting");
        }
    }
}
