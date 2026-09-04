using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class ExitViewModel : INotifyPropertyChanged
    {
        public ICommand SettingsCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand NoCommand { get; }
        public ICommand YesCommand { get; }
        private ProgramManager ProgramManager;

        public ExitViewModel()
        {
            SettingsCommand = new RelayCommand(param => OpenSettings(param));
            TrophyCommand = new RelayCommand(param => OpenTrophy(param));
            HomeCommand = new RelayCommand(param => OpenHome(param));
            NoCommand = new RelayCommand(param => OpenHome(param));
            YesCommand = new RelayCommand(param => ExitApplication());
            ProgramManager = ProgramManager.Instance;
        }

        private void OpenSettings(object? parameter)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void OpenTrophy(object? parameter)
        {
            var trophyWindow = new TrophyWindow();
            trophyWindow.Show();
        }

        private void OpenHome(object? parameter)
        {
            var homeWindow = new MainWindow();
            homeWindow.Show();
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        private static void CloseCurrentWindow(object? parameter)
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