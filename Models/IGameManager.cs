using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal interface IGameManager
    {
        Stack<Card> CardStack { get; set; }
        GameField GameField { get; set; }
        IPlayer Player1 { get; set; }
        IPlayer Player2 { get; set; }
        IPlayer CurrentPlayer { get; set; }

        void InitializeGameField();
        void SetRandomStartPlayer();
        Stack<Card> GenerateCardStack();
        Card GetCardFromCardStack();
        void DrawCard();
        bool CanDrawCard();
        void UpdateCurrentPlayer();
        bool IsGameOver(Card lastCardPlaced);
    }
}
