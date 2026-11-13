using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LabyrinthGame
{
    public class SinglePlayerViewModel : INotifyPropertyChanged
    {
        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand SaveCommand { get; }
        private ProgramManager ProgramManager;

        private bool _isDifficulty2Checked;
        public bool IsDifficulty2Checked
        {
            get => _isDifficulty2Checked;
            set
            {
                _isDifficulty2Checked = value;
                OnPropertyChanged(nameof(IsDifficulty2Checked));
                if (value) SelectedDifficulty = 1;
            }
        }

        private bool _isDifficulty3Checked;
        public bool IsDifficulty3Checked
        {
            get => _isDifficulty3Checked;
            set
            {
                _isDifficulty3Checked = value;
                OnPropertyChanged(nameof(IsDifficulty3Checked));
                if (value) SelectedDifficulty = 2;
            }
        }

        private int _selectedDifficulty;
        public int SelectedDifficulty
        {
            get => _selectedDifficulty;
            set
            {
                _selectedDifficulty = value;
                ProgramManager.BotDifficulty = SelectedDifficulty;
                OnPropertyChanged(nameof(SelectedDifficulty));
            }
        }

        public SinglePlayerViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            SaveCommand = new RelayCommand(param => OpenSaveWindow(param));
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

        private void OpenSaveWindow(object? parameter)
        {
            if (ProgramManager.BotDifficulty != 0)
            {
                ProgramManager.InitializeNewSingleplayerBotGame();
                var saveWindow = new FirstPlayingFieldWindow();
                saveWindow.Show();
                CloseCurrentWindow(parameter);
            }
            else
            {

            }
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