using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LabyrinthGame.ViewModels
{
    public class DrawViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand GameResultCommand { get; }


        public DrawViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            GameResultCommand = new RelayCommand(param => OpenGameResultWindow(param));
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
        private void OpenGameResultWindow(object? parameter)
        {
            var gameResultWindow = new GameResultWindow();
            gameResultWindow.Show();
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