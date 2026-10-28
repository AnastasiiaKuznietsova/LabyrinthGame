using LabyrinthGame.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace LabyrinthGame.ViewModels
{
    public class FirstPlayingFieldViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand RotateCardCommand { get; }
        public ICommand DrawCardCommand { get; }
        public ICommand TryToRemoveCardCommand { get; }
        private ProgramManager ProgramManager;


        public async Task UpdateFieldCards()
        {
            this.FieldCards.Clear();
            var (minX, maxX, minY, maxY) = ProgramManager.GameManager.GameField.GetOccupiedBounds();

            // Erweiterung des Bereichs um eine leere Zeile und Spalte an jeder Seite
            minX = Math.Max(0, minX - 1);
            maxX = Math.Min(99, maxX + 1);
            minY = Math.Max(0, minY - 1);
            maxY = Math.Min(99, maxY + 1);

            FieldCardsSizeX = maxX - minX + 1;
            FieldCardsSizeY = maxY - minY + 1;

            // Berechne die Größe des Quadrats
            int maxSize = Math.Max(FieldCardsSizeX, FieldCardsSizeY);
            FieldCardsSizeX = FieldCardsSizeY = maxSize;

            for (int y = 0; y < maxSize; y++)
            {
                for (int x = 0; x < maxSize; x++)
                {
                    int actualX = minX + x;
                    int actualY = minY + y;

                    if (actualX <= maxX && actualY <= maxY)
                    {
                        var card = ProgramManager.GameManager.GameField.GetCard(actualX, actualY);
                        if (card != null)
                        {
                            this.FieldCards.Add(card);
                        }
                        else
                        {
                            if (ProgramManager.SecretMode)
                            {
                                this.FieldCards.Add(new Card("empty", "pack://application:,,,/Pictures/Profiles/Alex.png"));
                            }
                            else
                            {
                                this.FieldCards.Add(new Card("empty", "pack://application:,,,/Pictures/Cards/empty.png"));
                            }
                        }
                    }
                    else
                    {
                        if (ProgramManager.SecretMode)
                        {
                            this.FieldCards.Add(new Card("empty", "pack://application:,,,/Pictures/Profiles/Alex.png"));
                        }
                        else
                        {
                            this.FieldCards.Add(new Card("empty", "pack://application:,,,/Pictures/Cards/empty.png"));
                        }
                    }
                }
            }

            OnPropertyChanged(nameof(FieldCards));
            OnPropertyChanged(nameof(FieldCardsSizeX));
            OnPropertyChanged(nameof(FieldCardsSizeY));
        }

        public async Task UpdatePlayerCardsAndScores()
        {
            Player1Cards.Clear();
            Player2Cards.Clear();

            foreach (Card card in ProgramManager.GameManager.Player1.CardsInHand)
            {
                Player1Cards.Add(card);
            }
            foreach (Card card in ProgramManager.GameManager.Player2.CardsInHand)
            {
                Player2Cards.Add(card);
            }

            OnPropertyChanged(nameof(Player1Score));
            OnPropertyChanged(nameof(Player2Score));
            OnPropertyChanged(nameof(CurrentPlayerName));
        }
        private async void RotateCard(object? parameter)
        {
            if (parameter is Card card)
            {
                card.Rotate();
                await ProgramManager.SendAction($"{Action.ROTATECARD}:{card.Id}");
            }
        }

        private void OnEndTurn()
        {
            this.DrawCard(null);
        }

        public async void DrawCard(object? parameter)
        {
            ProgramManager.DrawCard(this);
        }

        public void OnCardDrag(Card card, DependencyObject dragSource)
        {
            if (card != null && dragSource != null)
            {
                DragDrop.DoDragDrop(dragSource, new DataObject(card), DragDropEffects.Move);
            }
        }

        public async void OnCardDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(Card)))
            {
                var card = e.Data.GetData(typeof(Card)) as Card;
                var uiElement = e.OriginalSource as UIElement;

                if (card == null || uiElement == null)
                {
                    return;
                }

                var uniformGrid = FindParent<UniformGrid>(uiElement);
                if (uniformGrid == null)
                {
                    Debug.WriteLine("UniformGrid is null");
                    return;
                }

                // Berechne die Position relativ zum UniformGrid
                var position = e.GetPosition(uniformGrid);
                int targetY = (int)(position.Y / (uniformGrid.ActualHeight / uniformGrid.Rows));
                int targetX = (int)(position.X / (uniformGrid.ActualWidth / uniformGrid.Columns));

                Debug.WriteLine($"Row: {targetY}, Column: {targetX}");

                // Hier wird das Model verändert:
                var (minX, _, minY, _) = ProgramManager.GameManager.GameField.GetOccupiedBounds();

                Debug.WriteLine($"\nId: {card.Id}");
                if (ProgramManager.GameManager.GameField.GetAllPossibleCoordinatesWhereCardCanBePlaced(card).Contains((targetX + minX - 1, targetY + minY - 1).ToTuple()))
                {
                    ProgramManager.GameManager.GameField.AddCard(targetX + minX - 1, targetY + minY - 1, card);
                    await ProgramManager.SendAction($"{Action.PLACECARD}:{card.Id}:{targetX + minX - 1}:{targetY + minY - 1}");
                    UpdateFieldCards();

                    ProgramManager.GameManager.CurrentPlayer.CardsInHand.Remove(card);
                    UpdatePlayerCardsAndScores();

                    ProgramManager.GameManager.Player1.IsAbleToPlaceCard = false;
                    ProgramManager.GameManager.Player2.IsAbleToPlaceCard = false;
                    OnPropertyChanged(nameof(IsPlayer1CardsEnabled));
                    OnPropertyChanged(nameof(IsPlayer2CardsEnabled));

                    ProgramManager.GameManager.GameField.LastPlacedCard = card;
                    OnPropertyChanged(nameof(LastPlacedCard));

                    if (ProgramManager.GameManager.IsGameOver(ProgramManager.GameManager.GameField.LastPlacedCard))
                    {
                        this.EndGame(Application.Current.MainWindow);
                    }
                    else
                    {
                        // Delay for 1 second before showing the error message
                        await Task.Delay(1000);
                        // Display error message "Take second card"
                        ErrorMessage = "Take second card";
                        ErrorMessageVisibility = Visibility.Visible;
                        await Task.Delay(3000);
                        ErrorMessageVisibility = Visibility.Collapsed;
                        ErrorMessage = string.Empty;
                    }
                }
                else
                {
                    Debug.WriteLine($"Unable to place Card there");
                    ErrorMessage = "Unable to place map there, no path found";
                    ErrorMessageVisibility = Visibility.Visible;
                    await Task.Delay(3000);
                    ErrorMessageVisibility = Visibility.Collapsed;
                    ErrorMessage = string.Empty;

                }
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindParent<T>(parentObject);
            }
        }

        private async void TryToRemoveCard(object? parameter)
        {
            if (ProgramManager.GameManager.IsGameOver(ProgramManager.GameManager.GameField.LastPlacedCard))
            {
                this.EndGame(Application.Current.MainWindow);
            }

            if (parameter is Card cardToRemove && LastPlacedCard != null && !cardToRemove.Equals(LastPlacedCard))
            {
                Debug.WriteLine($"Last placed Card: {LastPlacedCard.Id}");
                Debug.WriteLine($"Card to remove: {cardToRemove.Id}");
                Debug.WriteLine("Card paths:");
                foreach (int path in cardToRemove.Paths.Keys)
                {
                    Debug.WriteLine($"{path}");
                }

                if (ProgramManager.GameManager.GameField.AreTheseCardsTreasuresConnected(LastPlacedCard, cardToRemove) && ProgramManager.GameManager.GameField.IsAllowedToRemoveCard(cardToRemove))
                {
                    (int x, int y) = ProgramManager.GameManager.GameField.GetCardCoordinate(cardToRemove);

                    if (ProgramManager.GameManager.GameField.GetCard(x, y) != null)
                    {
                        ProgramManager.GameManager.GameField.RemoveCard(x, y);
                        ProgramManager.GameManager.CurrentPlayer.AddScore();

                        if (ProgramManager.GameManager is OnlineMultiplayerGameManager onlineGameManager)
                        {
                            await ProgramManager.SendAction($"{Action.TAKECARD}:{x}:{y}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"There is no card in {x},{y}");
                    }
                }
                else
                {
                    ErrorMessage = "You cant pick up the card";
                    ErrorMessageVisibility = Visibility.Visible;
                    await Task.Delay(3000);
                    ErrorMessageVisibility = Visibility.Collapsed;
                    ErrorMessage = string.Empty;
                }
            }
            else
            {
                Debug.WriteLine("You cant pick up the same card you just placed!");
                ErrorMessage = "You cant pick up the same card you just placed!";
                ErrorMessageVisibility = Visibility.Visible;
                await Task.Delay(3000);
                ErrorMessageVisibility = Visibility.Collapsed;
                ErrorMessage = string.Empty;
            }

            Debug.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}, Name: {Thread.CurrentThread.Name}, Hintergrund-Thread: {Thread.CurrentThread.IsBackground}");
            UpdateFieldCards();
            UpdatePlayerCardsAndScores();
        }
    }
}