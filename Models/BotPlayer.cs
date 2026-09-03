using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class BotPlayer : IPlayer
    {
        public Profile Profile { get; set; }
        public int Difficulty { get; set; }
        public int Score { get; set; }
        public List<Card> CardsInHand { get; set; }
        public Card? SelectedCard { get; set; }
        public bool IsAbleToPlaceCard { get; set; }

        public BotPlayer(int pDifficulty, Card pCardInHand1 = null, Card pCardInHand2 = null)
        {
            this.Profile = new($"Bot {(pDifficulty == 1 ? "Easy" : "Hard")}");
            this.Difficulty = pDifficulty;
            this.Score = 0;
            this.CardsInHand = [pCardInHand1, pCardInHand2];
            this.SelectedCard = null;
            this.IsAbleToPlaceCard = false;
        }

        public async Task<BotAction> GetNextActionAsync(GameField gameField)
        {
            return await Task.Run(() => GetNextAction(gameField));
        }

        public BotAction GetNextAction(GameField gameField)
        {
            GameField gameFieldCopy = new(gameField);
            BotAction finalAction = null;
            List<Tuple<int, int>> allPossibleCoordinatesWhereCardCanBePlaced = new();
            List<BotAction> possibleActions = new();

            foreach ((Card possibleCardToPlace, int cardRotation) in this.GetAllPossibleCardsToPlaceInEveryRotation())
            {
                allPossibleCoordinatesWhereCardCanBePlaced = gameFieldCopy.GetAllPossibleCoordinatesWhereCardCanBePlaced(possibleCardToPlace);

                foreach ((int possibleXCoordinate, int possibleYCoordinate) in allPossibleCoordinatesWhereCardCanBePlaced)
                {
                    int evaluatedScoreOutcome = this.EvaluateScoreOutcome(gameField, possibleCardToPlace, possibleXCoordinate, possibleYCoordinate);
                    List<Card> cardsToRemove = this.GetCardsToRemove(gameField, possibleCardToPlace, possibleXCoordinate, possibleYCoordinate);
                    possibleCardToPlace.RotateBackToDefaultRotation();
                    possibleActions.Add(new(possibleCardToPlace, cardRotation, possibleXCoordinate, possibleYCoordinate, evaluatedScoreOutcome, cardsToRemove));
                }
            }

            if (this.Difficulty == 1)
            {
                finalAction = possibleActions.OrderBy(_ => Guid.NewGuid()).First();
            }
            else if (this.Difficulty == 2)
            {
                finalAction = possibleActions.OrderByDescending(action => action.PossibleScore).First();
            }
            else if (this.Difficulty == 3)
            {

            }

            return finalAction;
        }
        public int EvaluateScoreOutcome(GameField gameField, Card cardToPlace, int xCoordinate, int yCoordinate)
        {
            GameField gameFieldCopy = new(gameField);
            int scoreOutcome = 0;
            gameFieldCopy.AddCard(xCoordinate, yCoordinate, cardToPlace);

            foreach (Card possibleCardToRemove in gameFieldCopy.GetAllCards())
            {
                if (gameFieldCopy.AreTheseCardsTreasuresConnected(cardToPlace, possibleCardToRemove) && gameFieldCopy.IsAllowedToRemoveCard(possibleCardToRemove))
                {
                    scoreOutcome++;
                }
            }
            gameFieldCopy.RemoveCard(xCoordinate, yCoordinate);

            return scoreOutcome;
        }

        public List<Card> GetCardsToRemove(GameField gameField, Card cardToPlace, int xCoordinate, int yCoordinate)
        {
            GameField gameFieldCopy = new(gameField);
            List<Card> cardsToRemove = new();
            gameFieldCopy.AddCard(xCoordinate, yCoordinate, cardToPlace);

            foreach (Card possibleCardToRemove in gameFieldCopy.GetAllCards())
            {
                if (gameFieldCopy.AreTheseCardsTreasuresConnected(cardToPlace, possibleCardToRemove) && gameFieldCopy.IsAllowedToRemoveCard(possibleCardToRemove))
                {
                    cardsToRemove.Add(possibleCardToRemove);
                }
            }
            gameFieldCopy.RemoveCard(xCoordinate, yCoordinate);

            return cardsToRemove;
        }

        public List<Tuple<Card, int>> GetAllPossibleCardsToPlaceInEveryRotation()
        {
            List<Tuple<Card, int>> possibleCards = new();

            foreach (Card card in this.CardsInHand)
            {
                Card copiedCard = new(card);
                possibleCards.Add((copiedCard, 0).ToTuple());
                copiedCard = new(copiedCard);
                copiedCard.Rotate();
                possibleCards.Add((copiedCard, 1).ToTuple());
                copiedCard = new(copiedCard);
                copiedCard.Rotate();
                possibleCards.Add((copiedCard, 2).ToTuple());
                copiedCard = new(copiedCard);
                copiedCard.Rotate();
                possibleCards.Add((copiedCard, 3).ToTuple());
            }

            return possibleCards;
        }

        public void DrawCard(Card pCard)
        {
            if (CanDrawCard())
            {
                if (this.CardsInHand.Contains(null))
                {
                    this.CardsInHand.Remove(null);
                }

                this.CardsInHand.Add(pCard);
            }
        }

        public bool CanDrawCard()
        {
            return this.CardsInHand.Count < 2 || this.CardsInHand.Contains(null);
        }

        public void AddScore()
        {
            this.Score++;
        }
    }
}
