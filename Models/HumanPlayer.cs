using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    public class HumanPlayer : IPlayer
    {
        public Profile Profile { get; set; }
        public int Score { get; set; }
        public List<Card> CardsInHand { get; set; }
        public Card? SelectedCard { get; set; }
        public bool IsAbleToPlaceCard { get; set; }

        public HumanPlayer(
            Profile pProfile,
            Card pCardInHand1 = null,
            Card pCardInHand2 = null)
        {
            Profile = pProfile;
            Score = 0;
            CardsInHand = [pCardInHand1, pCardInHand2];
            SelectedCard = null;
            IsAbleToPlaceCard = false;
        }

        public void DrawCard(Card pCard)
        {
            if (CanDrawCard())
            {
                if (CardsInHand.Contains(null))
                {
                    CardsInHand.Remove(null);
                }

                CardsInHand.Add(pCard);
            }
        }

        public bool CanDrawCard()
        {
            return CardsInHand.Count < 2 || CardsInHand.Contains(null);
        }

        public void AddScore()
        {
            Score++;
        }

        Task<BotAction> IPlayer.GetNextActionAsync(GameField gameField)
        {
            return null;
        }
    }
}
