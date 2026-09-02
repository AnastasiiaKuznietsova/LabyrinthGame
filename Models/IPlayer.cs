using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal interface IPlayer
    {
        Profile Profile { get; set; }
        int Score { get; set; }
        List<Card> CardsInHand { get; set; }
        Card? SelectedCard { get; set; }
        bool IsAbleToPlaceCard { get; set; }

        void DrawCard(Card pCard);
        bool CanDrawCard();
        void AddScore();
        Task<BotAction> GetNextActionAsync(GameField gameField);
    }
}
