namespace NegativeEddy.LemonadeStand
{
    public partial class Game
    {
        public class Stand
        {
            public Stand(int id, decimal assets)
            {
                Id = id;
                Assets = assets;
            }

            /// <summary>
            /// The Id of the stand
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// Assets (cash on hand, in dollars)
            /// </summary>
            public decimal Assets { get; set; }

            /// <summary>
            /// if everything is ruined by thunderstorm
            /// originally G(i) : 0 = ruined, 1 = not ruined
            /// </summary>
            public bool RuinedByThunderstorm { get; set; }  

            /// <summary>
            /// true if the player is bankrupt and cannot continue
            /// </summary>
            public bool IsBankrupt { get; set; }

            /// <summary>
            /// number of glasses of lemonade made by player
            /// originally L(i)
            /// </summary>
            public int GlassesMade { get; set; } 

            /// <summary>
            /// Price charged for lemonade, per glass, in cents
            /// originally P(i)
            /// </summary>
            public int PricePerGlassCents { get; set; }  

            /// <summary>
            /// Number of signs made by player
            /// originally S(i)
            /// </summary>
            public int SignsMade { get; set; }
        }
    }
}
