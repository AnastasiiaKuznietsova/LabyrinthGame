using LabyrinthGame.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabyrinthGame.Models
{
    internal class ProgramManager
    {
        // Every ProgramManager manages a ProfileManager and a GameManager
        public ProfileManager ProfileManager { get; set; }
        public MusicManager MusicManager { get; set; }
        public IGameManager GameManager { get; set; }
        public Profile Profile1 { get; set; }
        public Profile Profile2 { get; set; }
        public int BotDifficulty { get; set; }
        public string IpAddressToConnectTo { get; set; }
        public bool SecretMode { get; set; }
        public bool IsBot { get; set; }
        private static ProgramManager _instance;


        public static ProgramManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProgramManager();
                }
                return _instance;
            }
        }

        public ProgramManager()
        {
            this.ProfileManager = new ProfileManager();
            ProfileManager.LoadProfiles();
            ProfileManager.SaveProfiles();
            this.MusicManager = new MusicManager();
            this.BotDifficulty = 0;
            this.SecretMode = false;
            this.IsBot = false;
        }


        // Here a new GameManager object is created to handle a single game between two players (a bot also counts as a player)
        // It needs 2 profiles as an input, that needs to be chosen by the user via the ui
        public void InitializeNewLocalMultiplayerGame()
        {
            this.GameManager = new LocalMultiplayerGameManager(this.Profile1, this.Profile2);
        }

        public void InitializeNewSingleplayerBotGame()
        {
            this.GameManager = new SingleplayerBotGameManager(this.Profile1, this.BotDifficulty);
        }

        public async void BotMakeTurn(FirstPlayingFieldViewModel viewModel)
        {
            GameField gameFieldCopy = new GameField(this.GameManager.GameField);
            BotAction actionToExecute = await this.GameManager.CurrentPlayer.GetNextActionAsync(gameFieldCopy);

            await Task.Delay(2000);
            Card cardToPlace = this.GameManager.CurrentPlayer.CardsInHand.FirstOrDefault(card => card.Id.Equals(actionToExecute.Card.Id));
            cardToPlace.RotateXManyTimes(actionToExecute.CardRotation);
            viewModel.UpdateFieldCards();
            if (this.GameManager is OnlineMultiplayerGameManager onlineMultiplayerGameManager)
            {
                for (int i = 0; i < actionToExecute.CardRotation; i++)
                {
                    await this.SendAction($"{Action.ROTATECARD}:{cardToPlace.Id}");
                    await Task.Delay(1000);
                }
            }
            await Task.Delay(1000);
            this.GameManager.GameField.AddCard(actionToExecute.XCoordinate, actionToExecute.YCoordinate, cardToPlace);
            this.GameManager.CurrentPlayer.CardsInHand.Remove(cardToPlace);
            viewModel.UpdatePlayerCardsAndScores();
            viewModel.UpdateFieldCards();
            if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager)
            {
                await this.SendAction($"{Action.PLACECARD}:{cardToPlace.Id}:{actionToExecute.XCoordinate}:{actionToExecute.YCoordinate}");
            }
            await Task.Delay(1000);

            Debug.WriteLine($"Amount of cards the bot wants to remove: {actionToExecute.CardsToRemove.Count}");
            if (actionToExecute.CardsToRemove != null)
            {
                foreach (Card cardToRemove in actionToExecute.CardsToRemove)
                {
                    if (this.GameManager.GameField.IsAllowedToRemoveCard(cardToRemove) && this.GameManager.GameField.AreTheseCardsTreasuresConnected(cardToPlace, cardToRemove))
                    {
                        Debug.WriteLine($"Bot removes card with Id: {cardToRemove.Id}");
                        (int xCoordinate, int yCoordinate) = this.GameManager.GameField.GetCardCoordinate(cardToRemove);
                        this.GameManager.GameField.RemoveCard(xCoordinate, yCoordinate);
                        this.GameManager.CurrentPlayer.AddScore();
                        viewModel.UpdateFieldCards();
                        viewModel.UpdatePlayerCardsAndScores();
                        if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager1)
                        {
                            await this.SendAction($"{Action.TAKECARD}:{xCoordinate}:{yCoordinate}");
                        }
                        await Task.Delay(1500);
                    }
                }
            }

            if (this.GameManager.IsGameOver(this.GameManager.GameField.LastPlacedCard))
            {
                viewModel.EndGame(Application.Current.MainWindow);
            }
            else
            {
                this.DrawCard(viewModel);
                viewModel.UpdateFieldCards();
                viewModel.UpdatePlayerCardsAndScores();
            }
        }

        public async void DrawCard(FirstPlayingFieldViewModel viewModel)
        {
            if (this.GameManager.CardStack.Count > 0)
            {
                if (this.GameManager.CanDrawCard())
                {
                    this.GameManager.DrawCard();
                    this.GameManager.UpdateCurrentPlayer();
                    viewModel.UpdatePlayerCardsAndScores();

                    if (this.GameManager.CurrentPlayer == this.GameManager.Player1)
                    {
                        this.GameManager.Player1.IsAbleToPlaceCard = true;
                        this.GameManager.Player2.IsAbleToPlaceCard = false;
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer1CardsEnabled));
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                    }
                    else
                    {
                        this.GameManager.Player1.IsAbleToPlaceCard = false;
                        this.GameManager.Player2.IsAbleToPlaceCard = true;
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer1CardsEnabled));
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                    }

                    GameManager.GameField.LastPlacedCard = null;

                    if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager && onlineGameManager.CurrentPlayer == onlineGameManager.Player2)
                    {
                        await this.SendAction($"{Action.ENDTURN}");
                        this.GameManager.Player2.IsAbleToPlaceCard = false;
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                    }
                    else if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager1 && onlineGameManager1.Player1 is BotPlayer && onlineGameManager1.CurrentPlayer == onlineGameManager1.Player1)
                    {
                        this.GameManager.Player2.IsAbleToPlaceCard = false;
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                        this.BotMakeTurn(viewModel);
                    }

                    if (this.GameManager is SingleplayerBotGameManager && this.GameManager.CurrentPlayer == this.GameManager.Player2)
                    {
                        this.GameManager.Player2.IsAbleToPlaceCard = false;
                        viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                        this.BotMakeTurn(viewModel);
                    }
                }
                else
                {
                    viewModel.ErrorMessage = "You already have 2 cards";
                    viewModel.ErrorMessageVisibility = Visibility.Visible;
                    await Task.Delay(3000);
                    viewModel.ErrorMessageVisibility = Visibility.Collapsed;
                    viewModel.ErrorMessage = string.Empty;
                }
            }
            else
            {
                viewModel.DeckButtonContent = "End turn ⇓";
                this.GameManager.UpdateCurrentPlayer();
                viewModel.UpdatePlayerCardsAndScores();

                if (this.GameManager.CurrentPlayer == this.GameManager.Player1)
                {
                    this.GameManager.Player1.IsAbleToPlaceCard = true;
                    this.GameManager.Player2.IsAbleToPlaceCard = false;
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer1CardsEnabled));
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                }
                else
                {
                    this.GameManager.Player1.IsAbleToPlaceCard = false;
                    this.GameManager.Player2.IsAbleToPlaceCard = true;
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer1CardsEnabled));
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                }

                GameManager.GameField.LastPlacedCard = null;

                if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager && onlineGameManager.CurrentPlayer == onlineGameManager.Player2)
                {
                    await this.SendAction($"{Action.ENDTURN}");
                    this.GameManager.Player2.IsAbleToPlaceCard = false;
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                }
                else if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager1 && onlineGameManager1.Player1 is BotPlayer && onlineGameManager1.CurrentPlayer == onlineGameManager1.Player1)
                {
                    this.GameManager.Player2.IsAbleToPlaceCard = false;
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                    this.BotMakeTurn(viewModel);
                }

                if (this.GameManager is SingleplayerBotGameManager && this.GameManager.CurrentPlayer == this.GameManager.Player2)
                {
                    this.GameManager.Player2.IsAbleToPlaceCard = false;
                    viewModel.OnPropertyChanged(nameof(viewModel.IsPlayer2CardsEnabled));
                    this.BotMakeTurn(viewModel);
                }
            }
            viewModel.UpdateFieldCards();
            viewModel.UpdatePlayerCardsAndScores();
        }

        #region Online Multiplayer
        public async Task<bool> InitializeNewOnlineMultiplayerGameAsHost()
        {
            this.GameManager = new OnlineMultiplayerGameManager(true, this.Profile1, IsBot);

            if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager)
            {
                if (await onlineGameManager.StartHost())
                {
                    await onlineGameManager.SyncInitialGameState();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> InitializeNewOnlineMultiplayerGameAsClient()
        {
            this.GameManager = new OnlineMultiplayerGameManager(false, this.Profile1, IsBot);

            if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager)
            {
                if (await onlineGameManager.ConnectToHost(this.IpAddressToConnectTo))
                {
                    await onlineGameManager.SyncInitialGameState();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public async Task SendAction(string actionToSend)
        {
            if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager)
            {
                await onlineGameManager.SendAction(actionToSend);
            }
        }

        public async Task<string> ReceiveAction()
        {
            if (this.GameManager is OnlineMultiplayerGameManager onlineGameManager)
            {
                return await onlineGameManager.ReceiveAction();
            }

            return string.Empty;
        }

        public IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    Debug.WriteLine(ip.ToString());
                    return ip;
                }
            }

            return null;
        }
        #endregion
    }

}