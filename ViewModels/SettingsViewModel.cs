using LabyrinthGame;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand RulesCommand { get; }
        public ICommand VolumeCommand { get; }
        public ICommand CloseCommand { get; }
        private ProgramManager ProgramManager;

        public SettingsViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            RulesCommand = new RelayCommand(param => OpenRulesWindow(param));
            VolumeCommand = new RelayCommand(param => OpenVolumeWindow(param));
            CloseCommand = new RelayCommand(param => OpenCloseWindow(param));
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

        private void OpenRulesWindow(object? parameter)
        {
            var rulesWindow = new RulesWindow();
            rulesWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenVolumeWindow(object? parameter)
        {
            var volumeWindow = new VolumeWindow();
            volumeWindow.Show();
            CloseCurrentWindow(parameter);
        }

        private void OpenCloseWindow(object? parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
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
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
