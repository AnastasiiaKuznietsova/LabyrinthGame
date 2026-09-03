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

        // Platziere 4 zufällige Karten aus dem CardStack in der Mitte des GameField (einmal zu Beginn jedes Spiels aufgerufen)
        public void InitializeGameField()
        {
            this.GameField.AddCard(50, 50, GetCardFromCardStack());
            this.GameField.AddCard(50, 51, GetCardFromCardStack());
            this.GameField.AddCard(51, 50, GetCardFromCardStack());
            this.GameField.AddCard(51, 51, GetCardFromCardStack());
        }

        // Setzt zufällig Player1 oder Player2 als CurrentPlayer.
        public void SetRandomStartPlayer()
        {
            Random random = new Random();
            this.CurrentPlayer = random.Next(1, 3) == 1 ? this.Player1 : this.Player2;
        }

        // Gibt die erste Karte aus dem CardStack zurück und löscht sie
        public Card GetCardFromCardStack()
        {
            var outputCard = this.CardStack.First();
            this.CardStack.Pop();
            return outputCard;
        }
    }
}
