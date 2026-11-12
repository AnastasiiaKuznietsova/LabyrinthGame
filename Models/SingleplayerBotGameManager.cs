using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class SingleplayerBotGameManager : IGameManager
    {
        // Jeder SingleplayerBotGameManager hat einen Kartenstapel, ein Spielfeld und 2 Spieler
        public Stack<Card> CardStack { get; set; }
        public GameField GameField { get; set; }
        public IPlayer Player1 { get; set; }
        public IPlayer Player2 { get; set; }
        public IPlayer CurrentPlayer { get; set; }

        // Der Konstruktor von SingleplayerBotGameManager benötigt ein Profil und die Bot-Schwierigkeit, um die Objekte Player1 und Player2 zu erstellen
        public SingleplayerBotGameManager(Profile pProfile1, int pBotDifficulty)
        {
            this.CardStack = GenerateCardStack();
            this.GameField = new();
            this.InitializeGameField();
            this.Player1 = new HumanPlayer(pProfile1, GetCardFromCardStack(), GetCardFromCardStack());
            this.Player2 = new BotPlayer(pBotDifficulty, GetCardFromCardStack(), GetCardFromCardStack());
            this.CurrentPlayer = this.Player1;
            this.CurrentPlayer.IsAbleToPlaceCard = true;
        }

        // Platziert 4 zufällige Karten aus dem Kartenstapel in der Mitte des Spielfelds (einmal zu Beginn jedes Spiels aufgerufen)
        public void InitializeGameField()
        {
            this.GameField.AddCard(50, 50, GetCardFromCardStack());
            this.GameField.AddCard(50, 51, GetCardFromCardStack());
            this.GameField.AddCard(51, 50, GetCardFromCardStack());
            this.GameField.AddCard(51, 51, GetCardFromCardStack());
        }

        // Setzt einen zufälligen Startspieler
        public void SetRandomStartPlayer()
        {
            Random random = new Random();
            this.CurrentPlayer = random.Next(1, 3) == 1 ? this.Player1 : this.Player2;
        }

        // Gibt einen Stapel mit 50 spezifischen Kartenobjekten in zufälliger Reihenfolge zurück
        public Stack<Card> GenerateCardStack()
        {
            // Initialisierung der Karten
            List<Card> cardList =
            [
                new("01;3458-67;Ring-3458+Krone-67","pack://application:,,,/Resources/Images/Cards/Card01.png"),
                new("02;23468;Smaragd-23468+Buch-23468", "pack://application:,,,/Resources/Images/Cards/Card02.png"),
                new("03;128-567;Gral-128+Einhorn-567", "pack://application:,,,/Resources/Images/Cards/Card03.png"),
                new("04;24-178;Ring-24+Fledermaus-178", "pack://application:,,,/Resources/Images/Cards/Card04.png"),
                new("05;238-56;Schlüssel-238+Kristall-56", "pack://application:,,,/Resources/Images/Cards/Card05.png"),
                new("06;1248-67;Horn-1248+Gral-67", "pack://application:,,,/Resources/Images/Cards/Card06.png"),
                new("07;248;Buch-248+Geist-248", "pack://application:,,,/Resources/Images/Cards/Card07.png"),
                new("08;1356;Horn-1356+Schatz-1356", "pack://application:,,,/Resources/Images/Cards/Card08.png"),
                new("09;1347;Geist-1347+Vogeldrache-1347", "pack://application:,,,/Resources/Images/Cards/Card09.png"),
                new("10;158-67;Wunderlampe-158+Pullovertyp-67", "pack://application:,,,/Resources/Images/Cards/Card10.png"),
                new("11;348;Wunderlampe-348+Narr-348", "pack://application:,,,/Resources/Images/Cards/Card11.png"),
                new("12;235-78;Goldbeutel-235+Narr-78", "pack://application:,,,/Resources/Images/Cards/Card12.png"),
                new("13;16-235;Wunderlampe-235+Vogeldrache-16", "pack://application:,,,/Resources/Images/Cards/Card13.png"),
                new("14;16-23-45;Ring-23+Messer-16", "pack://application:,,,/Resources/Images/Cards/Card14.png"),
                new("15;1678-24;Wunderlampe-24+Einhorn-1678", "pack://application:,,,/Resources/Images/Cards/Card15.png"),
                new("16;28-467;Fledermaus-28+Kerzenständer-467", "pack://application:,,,/Resources/Images/Cards/Card16.png"),
                new("17;138-57;Buch-138+Schatz-57", "pack://application:,,,/Resources/Images/Cards/Card17.png"),
                new("18;1246;Narr-1246+Rüstung-1246", "pack://application:,,,/Resources/Images/Cards/Card18.png"),
                new("19;1468;Kobold-1468+Goldbeutel-1468", "pack://application:,,,/Resources/Images/Cards/Card19.png"),
                new("20;3468;Troll-3468+Goldbeutel-3468", "pack://application:,,,/Resources/Images/Cards/Card20.png"),
                new("21;2578;Kobold-2578+Schlüssel-2578", "pack://application:,,,/Resources/Images/Cards/Card21.png"),
                new("22;1268;Maus-1268+Goldbeutel-1268", "pack://application:,,,/Resources/Images/Cards/Card22.png"),
                new("23;258;Ring-258+Smaragd-258", "pack://application:,,,/Resources/Images/Cards/Card23.png"),
                new("24;2357;Kristall-2357+Fledermaus-2357", "pack://application:,,,/Resources/Images/Cards/Card24.png"),
                new("25;1357;Schatz-1357+Einhorn-1357", "pack://application:,,,/Resources/Images/Cards/Card25.png"),
                new("26;134;Kristall-134+Kerzenständer-134", "pack://application:,,,/Resources/Images/Cards/Card26.png"),
                new("27;13-456;Kobold-13+Gral-456", "pack://application:,,,/Resources/Images/Cards/Card27.png"),
                new("28;236-45;Schatz-236+Troll-45", "pack://application:,,,/Resources/Images/Cards/Card28.png"),
                new("29;18-26;Geist-18+Kerzenständer-26", "pack://application:,,,/Resources/Images/Cards/Card29.png"),
                new("30;1235;Rüstung-1235+Kristall-1235", "pack://application:,,,/Resources/Images/Cards/Card30.png"),
                new("31;1347;Drache-1347+Maus-1347", "pack://application:,,,/Resources/Images/Cards/Card31.png"),
                new("32;17-23-45;Karte-23+Vogeldrache-17", "pack://application:,,,/Resources/Images/Cards/Card32.png"),
                new("33;136;Messer-136+Horn-136", "pack://application:,,,/Resources/Images/Cards/Card33.png"),
                new("34;238-57;Drache-238+Schlüssel-57", "pack://application:,,,/Resources/Images/Cards/Card34.png"),
                new("35;2468;Buch-2468+Fledermaus-2468", "pack://application:,,,/Resources/Images/Cards/Card35.png"),
                new("36;235-68;Geist-235+Karte-68", "pack://application:,,,/Resources/Images/Cards/Card36.png"),
                new("37;146-23;Krone-146+Gral-23", "pack://application:,,,/Resources/Images/Cards/Card37.png"),
                new("38;15-23;Troll-15+Schlüssel-23", "pack://application:,,,/Resources/Images/Cards/Card38.png"),
                new("39;2478;Karte-2478+Rüstung-2478", "pack://application:,,,/Resources/Images/Cards/Card39.png"),
                new("40;18-24-67;Vogeldrache-24+Drache-67", "pack://application:,,,/Resources/Images/Cards/Card40.png"),
                new("41;246;Karte-246+Messer-246", "pack://application:,,,/Resources/Images/Cards/Card41.png"),
                new("42;1457-23;Smaragd-1457+Narr-23", "pack://application:,,,/Resources/Images/Cards/Card42.png"),
                new("43;2378;Pullovertyp-2378+Krone-2378", "pack://application:,,,/Resources/Images/Cards/Card43.png"),
                new("44;135;Drache-135+Horn-135", "pack://application:,,,/Resources/Images/Cards/Card44.png"),
                new("45;135;Maus-135+Einhorn-135", "pack://application:,,,/Resources/Images/Cards/Card45.png"),
                new("46;358-67;Troll-358+Kobold-67", "pack://application:,,,/Resources/Images/Cards/Card46.png"),
                new("47;13-4567;Maus-13+Pullovertyp-4567", "pack://application:,,,/Resources/Images/Cards/Card47.png"),
                new("48;18-2467;Pullovertyp-18+Messer-2467", "pack://application:,,,/Resources/Images/Cards/Card48.png"),
                new("49;123-57;Rüstung-123+Kerzenständer-57", "pack://application:,,,/Resources/Images/Cards/Card49.png"),
                new("50;234-68;Krone-234+Smaragd-68", "pack://application:,,,/Resources/Images/Cards/Card50.png")
            ];

            // Mischt die Karten und legt sie in den Stapel
            Stack<Card> outputCardStack = new(cardList.OrderBy(c => Guid.NewGuid()));

            // Gibt den Stapel zurück 
            return outputCardStack;
        }

        // Gibt die erste Karte auf dem Kartenstapel zurück und löscht sie
        public Card GetCardFromCardStack()
        {
            var outputCard = this.CardStack.First();
            this.CardStack.Pop();
            return outputCard;
        }

        // Zieht eine Karte für den aktuellen Spieler
        public void DrawCard()
        {
            CurrentPlayer.DrawCard(GetCardFromCardStack());
        }

        // Überprüft, ob der aktuelle Spieler eine Karte ziehen kann // Перевіряє, чи може поточний гравець тягнути карту
        public bool CanDrawCard()
        {
            return CurrentPlayer.CanDrawCard();
        }

        // Aktualisiert den aktuellen Spieler 
        public void UpdateCurrentPlayer()
        {
            this.CurrentPlayer = this.CurrentPlayer == this.Player1 ? this.Player2 : this.Player1;
        }

        // Überprüft, ob das Spiel vorbei ist
        public bool IsGameOver(Card lastCardPlaced)
        {
            return this.CardStack.Count == 0 && this.Player1.CardsInHand.Count == 0 && this.Player2.CardsInHand.Count == 0;
        }
    }
}
