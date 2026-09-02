using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class BotAction
    {
        public Card Card { get; set; }
        public int CardRotation { get; set; }
        public int XCoordinate { get; set; }
        public int YCoordinate { get; set; }
        public int PossibleScore { get; set; }
        public List<Card> CardsToRemove { get; set; }

        public BotAction(
            Card pCard,
            int pCardRotation,
            int pXCoordinate,
            int pYCoordinate,
            int pPossibleScore,
            List<Card> pCardsToRemove)
        {
            Card = pCard;
            CardRotation = pCardRotation;
            XCoordinate = pXCoordinate;
            YCoordinate = pYCoordinate;
            PossibleScore = pPossibleScore;
            CardsToRemove = pCardsToRemove;
        }
    }
}
