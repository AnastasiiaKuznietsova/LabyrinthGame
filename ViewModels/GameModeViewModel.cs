using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class GameModeViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand SinglePlayerCommand { get; }
        public ICommand MultiPlayerLocallyCommand { get; }
        public ICommand MultiPlayerOnlineCommand { get; }
        private ProgramManager ProgramManager;

        public GameModeViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            SinglePlayerCommand = new RelayCommand(param => OpenSinglePlayerWindow(param));
            MultiPlayerLocallyCommand = new RelayCommand(param => OpenMultiPlayerLocallyWindow(param));
            MultiPlayerOnlineCommand = new RelayCommand(param => OpenMultiPlayerOnlineWindow(param));
            ProgramManager = ProgramManager.Instance;
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

        private void OpenSinglePlayerWindow(object? parameter)
        {
            var singlePlayerWindow = new SinglePlayerWindow();
            singlePlayerWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenMultiPlayerLocallyWindow(object? parameter)
        {
            var multiPlayerLocallyWindow = new MultiPlayerLocallyWindow();
            multiPlayerLocallyWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenMultiPlayerOnlineWindow(object? parameter)
        {
            var multiPlayerOnlineWindow = new MultiPlayerOnlineWindow();
            multiPlayerOnlineWindow.Show();
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
