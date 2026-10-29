using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class GameResultViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SettingsCommand { get; }

        private ProgramManager ProgramManager;

        public GameResultViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            ProgramManager = ProgramManager.Instance;

            Player1Name = ProgramManager.GameManager.Player1.Profile.Nickname;
            Player2Name = ProgramManager.GameManager.Player2.Profile.Nickname;
            OnPropertyChanged(nameof(Player1Name));
            OnPropertyChanged(nameof(Player2Name));
        }

        private string _player1Name;
        public string Player1Name
        {
            get { return _player1Name; }
            set
            {
                _player1Name = value;
                OnPropertyChanged(nameof(Player1Name));
            }
        }

        private string _player2Name;
        public string Player2Name
        {
            get { return _player2Name; }
            set
            {
                _player2Name = value;
                OnPropertyChanged(nameof(Player2Name));
            }
        }

        public int Player1Score
        {
            get
            {
                return ProgramManager.GameManager.Player1.Score;
            }
        }

        public int Player2Score
        {
            get
            {
                return ProgramManager.GameManager.Player2.Score;
            }
        }

        private void OpenHomeWindow(object? parameter)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
            CloseCurrentWindow(parameter);
        }
        private void OpenTrophyWindow(object? parameter)
        {
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenExitWindow(object? parameter)
        {
            var exitWindow = new ExitWindow();
            exitWindow.Show();
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