using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class LocalMultiplayerGameManager
    {
        // Jeder LocalMultiplayerGameManager hat einen CardStack, ein GameField und 2 Spieler
        public Stack<Card> CardStack { get; set; }
        public GameField GameField { get; set; }
        public IPlayer Player1 { get; set; }
        public IPlayer Player2 { get; set; }
        public IPlayer CurrentPlayer { get; set; }

        // Der Konstruktor von LocalMultiplayerGameManager benötigt 2 Profile, um die Objekte Player1 und Player2 zu erstellen
        public LocalMultiplayerGameManager(Profile pProfile1, Profile pProfile2)
        {
            this.CardStack = GenerateCardStack();
            this.GameField = new();
            this.InitializeGameField();
            this.Player1 = new HumanPlayer(pProfile1, GetCardFromCardStack(), GetCardFromCardStack());
            this.Player2 = new HumanPlayer(pProfile2, GetCardFromCardStack(), GetCardFromCardStack());
            this.SetRandomStartPlayer();
            this.CurrentPlayer.IsAbleToPlaceCard = true;
        }
    }
}
