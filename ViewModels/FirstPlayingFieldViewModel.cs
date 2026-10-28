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
    }
}