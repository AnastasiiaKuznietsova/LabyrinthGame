using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ICommand TrophyCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand ChangeProfileCommand { get; }
        public ICommand SecretCommand { get; }
        private ProgramManager ProgramManager;

        public MainViewModel()
        {
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            ChangeProfileCommand = new RelayCommand(param => OpenChangeProfileWindow(param));
            SecretCommand = new RelayCommand(param => EnableSecret(param));
            ProgramManager = ProgramManager.Instance;
        }

        private void EnableSecret(object? parameter)
        {
            ProgramManager.SecretMode = true;
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
        private void OpenChangeProfileWindow(object? parameter)
        {
            var changeProfileWindow = new ChangeProfileWindow();
            changeProfileWindow.Show();
            CloseCurrentWindow(parameter);
        }
        private void OpenTrophyWindow(object? parameter)
        {
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
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
