using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.Models
{
    internal class OnlineMultiplayerGameManager : IGameManager
    {
        // Every LocalMultiplayerGameManager has a CardStack, a GameField and 2 Players
        public Stack<Card> CardStack { get; set; }
        public GameField GameField { get; set; }
        public IPlayer Player1 { get; set; }
        public IPlayer Player2 { get; set; }
        public IPlayer CurrentPlayer { get; set; }
        public bool IsHost { get; set; }
        private TcpListener Server { get; set; }
        private TcpClient Client { get; set; }
        private NetworkStream Stream { get; set; }
        private ProgramManager ProgramManager;

        public event System.Action EndTurn;
        public void ExecuteEndTurn()
        {
            EndTurn?.Invoke();
        }


        // The LocalMultiplayerGameManager Constructor needs 2 Profiles in order to create the Player1 and Player2 objects
        public OnlineMultiplayerGameManager(bool pIsHost, Profile pProfile, bool pIsBot)
        {
            ProgramManager = ProgramManager.Instance;
            this.IsHost = pIsHost;
            this.GameField = new();
            this.CardStack = new();
            this.Player1 = pIsBot ? new BotPlayer(2) : new HumanPlayer(pProfile);

            if (this.IsHost)
            {
                this.CardStack = this.GenerateCardStack();
            }
        }


        // Place 4 random card from the CardStack in the middle of the GameField (once called at the start of every game) 
        public void InitializeGameField()
        {
            this.GameField.AddCard(49, 49, GetCardFromCardStack());
            this.GameField.AddCard(50, 49, GetCardFromCardStack());
            this.GameField.AddCard(49, 50, GetCardFromCardStack());
            this.GameField.AddCard(50, 50, GetCardFromCardStack());
        }

        public void SetRandomStartPlayer()
        {
            Random random = new Random();
            this.CurrentPlayer = random.Next(1, 3) == 1 ? this.Player1 : this.Player2;
        }

        // Returns a Stack full of 50 specific Card objects (which are created here) in a random order
        public Stack<Card> GenerateCardStack()
        {
            // Initialization of the cards
            List<Card> cardList =
            [
                new("01;3458-67;Ring-3458+Krone-67","pack://application:,,,/Resources/Images/Cards/Card01.png"),
                new("02;234-68;Smaragd-68+Krone-234", "pack://application:,,,/Resources/Images/Cards/Card02.png"),
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
                new("33;136;Messer-136+Horn-136", "pack://application:,,,/PicResources/Imagestures/Cards/Card33.png"),
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
                new("50;1348;Buch-1348+Smaragd-1348", "pack://application:,,,/Resources/Images/Cards/Card50.png")
            ];

            // Shuffle cards and put them in the stack
            Stack<Card> outputCardStack = new(cardList.OrderBy(c => Guid.NewGuid()));

            // Return stack
            return outputCardStack;
        }

        // Returns the first Card on the CardStack and deletes it
        public Card GetCardFromCardStack()
        {
            var outputCard = this.CardStack.First();
            this.CardStack.Pop();
            return outputCard;
        }

        public void DrawCard()
        {
            CurrentPlayer.DrawCard(GetCardFromCardStack());
        }

        public bool CanDrawCard()
        {
            return CurrentPlayer.CanDrawCard();
        }

        public void UpdateCurrentPlayer()
        {
            this.CurrentPlayer = this.CurrentPlayer == this.Player1 ? this.Player2 : this.Player1;
        }

        public bool IsGameOver(Card lastCardPlaced)
        {
            return this.CardStack.Count == 0 && this.Player1.CardsInHand.Count == 0 && this.Player2.CardsInHand.Count == 0;
        }

        public void InitializeGameState()
        {
            if (IsHost)
            {
                this.InitializeGameField();
                this.Player1.DrawCard(GetCardFromCardStack());
                this.Player1.DrawCard(GetCardFromCardStack());
                this.Player2.DrawCard(GetCardFromCardStack());
                this.Player2.DrawCard(GetCardFromCardStack());
                this.CurrentPlayer = this.Player1;
                this.Player1.IsAbleToPlaceCard = true;
                this.Player2.IsAbleToPlaceCard = false;
            }
            else
            {
                this.InitializeGameField();
                this.Player2.DrawCard(GetCardFromCardStack());
                this.Player2.DrawCard(GetCardFromCardStack());
                this.Player1.DrawCard(GetCardFromCardStack());
                this.Player1.DrawCard(GetCardFromCardStack());
                this.CurrentPlayer = this.Player2;
                this.Player1.IsAbleToPlaceCard = false;
                this.Player2.IsAbleToPlaceCard = false;
            }
        }

        #region Networking
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

        public async Task<bool> StartHost()
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Debug.WriteLine("Starting server...");
                    this.Server = new TcpListener(GetLocalIPAddress(), 5000);
                    this.Server.Start();
                    Debug.WriteLine("Server started");

                    Debug.WriteLine("Waiting for client to connect...");
                    this.Client = await this.Server.AcceptTcpClientAsync();
                    this.Stream = this.Client.GetStream();
                    Debug.WriteLine("Client connected");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt == maxAttempts)
                    {
                        Debug.WriteLine("Max attempts reached. Server could not be started.");
                        return false;
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }
            return false;
        }

        public async Task<bool> ConnectToHost(string hostIp)
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Debug.WriteLine("Connecting to host...");
                    this.Client = new TcpClient();
                    await this.Client.ConnectAsync(hostIp, 5000);
                    this.Stream = this.Client.GetStream();
                    Debug.WriteLine($"Connected to host {hostIp}");
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt == maxAttempts)
                    {
                        Debug.WriteLine("Max attempts reached. Connection failed.");
                        return false;
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }
            return false;
        }

        public async Task EndConnection()
        {
            if (this.IsHost)
            {
                this.Stream.Close();
                this.Client.Close();
                this.Server.Stop();
            }
            else
            {
                this.Stream.Close();
                this.Client.Close();
            }
            Debug.WriteLine("Connection closed");
        }

        public async Task SendAction(string action)
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Debug.WriteLine("Sending message...");
                    byte[] actionData = Encoding.ASCII.GetBytes(action);
                    await this.Stream.WriteAsync(actionData, 0, actionData.Length);
                    Debug.WriteLine($"Message `{action}` sent successfully");
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt == maxAttempts)
                    {
                        Debug.WriteLine("Max attempts reached. Could not send message.");
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }
        }
        public async Task<string> ReceiveAction()
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    Debug.WriteLine("Waiting for message...");
                    byte[] buffer = new byte[1024];
                    int bytesRead = await this.Stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                    string messageReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.WriteLine($"Player sent you this message: {messageReceived}");
                    return messageReceived;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Attempt {attempt} failed: {ex.Message}");
                    if (attempt == maxAttempts)
                    {
                        Debug.WriteLine("Max attempts reached. Could not receive message.");
                    }
                    else
                    {
                        await Task.Delay(2000);
                    }
                }
            }

            return string.Empty;
        }
        public async Task SyncInitialGameState()
        {
            if (this.IsHost)
            {
                await this.SendAction($"{Action.SETOPPONENTPLAYER}:{this.Player1.Profile.Nickname}");
                await this.ReceiveAndExecuteAction();
                await this.SendAction($"{Action.SETCARDSTACK}:{string.Join(":", CardStack.Select(card => card.Id))}");
                this.InitializeGameState();
            }
            else
            {
                await this.ReceiveAndExecuteAction();
                await this.SendAction($"{Action.SETOPPONENTPLAYER}:{this.Player1.Profile.Nickname}");
                await this.ReceiveAndExecuteAction();
                this.InitializeGameState();
            }
        }

        public async Task ReceiveAndExecuteAction()
        {
            string actionToExecute = await this.ReceiveAction();

            if (Enum.TryParse(actionToExecute.Split(":")[0], out Action action))
            {
                switch (action)
                {
                    case Action.SETOPPONENTPLAYER:
                        Debug.WriteLine("Setting opponent player...");
                        this.Player2 = new HumanPlayer(new Profile(actionToExecute.Split(":")[1]));
                        break;
                    case Action.SETCARDSTACK:
                        Debug.WriteLine("Setting card stack...");
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
                new("50;1348;Buch-1348+Smaragd-1348", "pack://application:,,,/Resources/Images/Cards/Card50.png")
            ];
                        string[] ids = string.Join(":", actionToExecute.Split(":").Skip(1)).Split(":");

                        foreach (string id in ids.Reverse())
                        {
                            this.CardStack.Push(cardList.FirstOrDefault(card => card.Id.StartsWith(id)));
                        }
                        break;
                    case Action.PLACECARD:
                        Debug.WriteLine("Placing card...");
                        Card cardToPlace = CurrentPlayer.CardsInHand.FirstOrDefault(card => card.Id == actionToExecute.Split(":")[1]);
                        int xCoordinate = Convert.ToInt32(actionToExecute.Split(":")[2]);
                        int yCoordinate = Convert.ToInt32(actionToExecute.Split(":")[3]);
                        this.GameField.AddCard(xCoordinate, yCoordinate, cardToPlace);
                        this.CurrentPlayer.CardsInHand.Remove(cardToPlace);
                        break;
                    case Action.ROTATECARD:
                        Debug.WriteLine("Rotating card...");
                        CurrentPlayer.CardsInHand.FirstOrDefault(card => card.Id == actionToExecute.Split(":")[1]).Rotate();
                        break;
                    case Action.TAKECARD:
                        Debug.WriteLine("Taking card...");
                        (int xCoordinateToTake, int yCoordinateToTake) = (int.Parse(actionToExecute.Split(':')[1]), int.Parse(actionToExecute.Split(':')[2]));
                        this.GameField.RemoveCard(xCoordinateToTake, yCoordinateToTake);
                        this.CurrentPlayer.AddScore();
                        break;
                    case Action.ENDTURN:
                        Debug.WriteLine("Ending turn...");
                        this.ExecuteEndTurn();
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid action.");
            }
        }

        #endregion
    }

    public enum Action
    {
        SETOPPONENTPLAYER,
        SETCARDSTACK,
        PLACECARD,
        ROTATECARD,
        TAKECARD,
        ENDTURN
    }
}