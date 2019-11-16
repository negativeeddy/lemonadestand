namespace NegativeEddy.LemonadeStand
{
    public partial class Game
    {
        public class Stand
        {
            public Stand(decimal assets)
            {
                Assets = assets;
            }

            /// <summary>
            /// Assets (cash on hand, in dollars)
            /// </summary>
            public decimal Assets { get; set; }

            /// <summary>
            /// if everything is ruined by thunderstorm
            /// originally G(i) : 0 = ruined, 1 = not ruined
            /// </summary>
            public bool RuinedByThunderstorm { get; set; }  

            public bool IsBankrupt { get; set; }
            public int L_GlassesMade { get; set; } // L(i): number of glasses of lemonade made by player i 
            public int P_PricePerGlassCents { get; set; } //P(i): Price charged for lemonade, per glass, in cents 
            public int S_SignsMade { get; set; } // S(i): Number of signs made by player i 
            public decimal H { get; set; } // H(i): apparently intended to relate to storms, but never assigned a value 
        }
    }
}
