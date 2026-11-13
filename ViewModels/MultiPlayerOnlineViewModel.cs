using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame
{
    public class MultiPlayerOnlineViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand HostGameCommand { get; }
        public ICommand JoinGameCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand SwitchPlayerCommand { get; }

        private ProgramManager ProgramManager;

        public string InputIpAddress
        {
            get => ProgramManager.IpAddressToConnectTo;
            set
            {
                ProgramManager.IpAddressToConnectTo = value;
                OnPropertyChanged(nameof(InputIpAddress));
            }
        }

        private bool _isJoinGameButtonHitVisible;
        public bool IsJoinGameButtonHitVisible
        {
            get => _isJoinGameButtonHitVisible;
            set
            {
                _isJoinGameButtonHitVisible = value;
                OnPropertyChanged(nameof(IsJoinGameButtonHitVisible));
            }
        }

        private bool _isHostGameButtonHitVisible;
        public bool IsHostGameButtonHitVisible
        {
            get => _isHostGameButtonHitVisible;
            set
            {
                _isHostGameButtonHitVisible = value;
                OnPropertyChanged(nameof(IsHostGameButtonHitVisible));
            }
        }

        private bool _isStartGameButtonHitVisible;
        public bool IsStartGameButtonHitVisible
        {
            get => _isStartGameButtonHitVisible;
            set
            {
                _isStartGameButtonHitVisible = value;
                OnPropertyChanged(nameof(IsStartGameButtonHitVisible));
            }
        }

        private bool _isStartGameButtonVisible;
        public bool IsStartGameButtonVisible
        {
            get => _isStartGameButtonVisible;
            set
            {
                _isStartGameButtonVisible = value;
                OnPropertyChanged(nameof(IsStartGameButtonVisible));
                OnPropertyChanged(nameof(StartGameButtonVisibility));
            }
        }
        public Visibility StartGameButtonVisibility => IsStartGameButtonVisible ? Visibility.Visible : Visibility.Collapsed;

        private string _hostGameButtonContent;
        public string HostGameButtonContent
        {
            get => _hostGameButtonContent;
            set
            {
                _hostGameButtonContent = value;
                OnPropertyChanged(nameof(HostGameButtonContent));
            }
        }

        private string _joinGameButtonContent;
        public string JoinGameButtonContent
        {
            get => _joinGameButtonContent;
            set
            {
                _joinGameButtonContent = value;
                OnPropertyChanged(nameof(JoinGameButtonContent));
            }
        }

        private bool _isSwitchPlayerButtonVisible;
        public bool IsSwitchPlayerButtonVisible
        {
            get => _isSwitchPlayerButtonVisible;
            set
            {
                _isSwitchPlayerButtonVisible = value;
                OnPropertyChanged(nameof(IsSwitchPlayerButtonVisible));
                OnPropertyChanged(nameof(SwitchPlayerButtonVisibility));
            }
        }
        public Visibility SwitchPlayerButtonVisibility => IsSwitchPlayerButtonVisible ? Visibility.Visible : Visibility.Collapsed;

        private bool _isSwitchPlayerButtonHitVisible;
        public bool IsSwitchPlayerButtonHitVisible
        {
            get => _isSwitchPlayerButtonHitVisible;
            set
            {
                _isSwitchPlayerButtonHitVisible = value;
                OnPropertyChanged(nameof(IsSwitchPlayerButtonHitVisible));
            }
        }

        private string _switchPlayerButtonContent;
        public string SwitchPlayerButtonContent
        {
            get => _switchPlayerButtonContent;
            set
            {
                _switchPlayerButtonContent = value;
                OnPropertyChanged(nameof(SwitchPlayerButtonContent));
            }
        }


        public MultiPlayerOnlineViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            HostGameCommand = new RelayCommand(param => HostGame(param));
            JoinGameCommand = new RelayCommand(param => JoinGame(param));
            StartGameCommand = new RelayCommand(param => StartGame(param));
            SwitchPlayerCommand = new RelayCommand(param => SwitchPlayer(param));

            ProgramManager = ProgramManager.Instance;

            this.IsStartGameButtonVisible = false;
            this.IsStartGameButtonHitVisible = false;
            this.IsHostGameButtonHitVisible = true;
            this.IsJoinGameButtonHitVisible = true;
            this.HostGameButtonContent = "Host Game";
            this.JoinGameButtonContent = "Join Game";
            this.IsSwitchPlayerButtonVisible = true;
            this.IsSwitchPlayerButtonHitVisible = true;
            this.SwitchPlayerButtonContent = "Switch to Bot";
        }


        private async void HostGame(object? parameter)
        {
            this.IsHostGameButtonHitVisible = false;
            this.IsJoinGameButtonHitVisible = false;
            this.IsSwitchPlayerButtonVisible = false;
            this.IsSwitchPlayerButtonHitVisible = false;
            this.InputIpAddress = this.ProgramManager.GetLocalIPAddress().ToString();
            this.HostGameButtonContent = "Waiting for client...";
            if (await ProgramManager.InitializeNewOnlineMultiplayerGameAsHost())
            {
                this.HostGameButtonContent = $"Connected to {ProgramManager.GameManager.Player2.Profile.Nickname}";
                this.IsStartGameButtonVisible = true;
                this.IsStartGameButtonHitVisible = true;
            }
            else
            {
                this.HostGameButtonContent = "Host Game";
                this.IsHostGameButtonHitVisible = true;
                this.IsJoinGameButtonHitVisible = true;
                this.IsSwitchPlayerButtonVisible = true;
                this.IsSwitchPlayerButtonHitVisible = true;
                this.InputIpAddress = string.Empty;
            }
        }

        private async void JoinGame(object? parameter)
        {
            if (ProgramManager.IpAddressToConnectTo != string.Empty)
            {
                this.IsHostGameButtonHitVisible = false;
                this.IsJoinGameButtonHitVisible = false;
                this.IsSwitchPlayerButtonVisible = false;
                this.IsSwitchPlayerButtonHitVisible = false;
                this.JoinGameButtonContent = "Waiting for host...";
                if (await ProgramManager.InitializeNewOnlineMultiplayerGameAsClient())
                {
                    this.JoinGameButtonContent = $"Connected to {ProgramManager.GameManager.Player2.Profile.Nickname}";
                    this.IsStartGameButtonVisible = true;
                    this.IsStartGameButtonHitVisible = true;
                }
                else
                {
                    this.JoinGameButtonContent = "Join Game";
                    this.IsHostGameButtonHitVisible = true;
                    this.IsJoinGameButtonHitVisible = true;
                    this.IsSwitchPlayerButtonVisible = true;
                    this.IsSwitchPlayerButtonHitVisible = true;
                    this.InputIpAddress = string.Empty;
                }
            }
        }

        private async void SwitchPlayer(object? parameter)
        {
            if (this.SwitchPlayerButtonContent == "Switch to Bot")
            {
                this.ProgramManager.IsBot = true;
                this.SwitchPlayerButtonContent = "Switch to Human";
            }
            else
            {
                this.ProgramManager.IsBot = false;
                this.SwitchPlayerButtonContent = "Switch to Bot";
            }
        }

        private void StartGame(object? parameter)
        {
            var saveWindow = new FirstPlayingFieldWindow();
            saveWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenHomeWindow(object? parameter)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenExitWindow(object? parameter)
        {
            var exitWindow = new ExitWindow();
            exitWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenTrophyWindow(object? parameter)
        {
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenSettingsWindow(object? parameter)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void CloseCurrentWindow(object? parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}