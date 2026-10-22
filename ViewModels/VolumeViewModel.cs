using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame.ViewModels
{
    public class VolumeViewModel : INotifyPropertyChanged
    {
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private int currentVolume = 50;
        public int CurrentVolume
        {
            get => currentVolume;
            set
            {
                currentVolume = value;
                OnPropertyChanged(nameof(CurrentVolume));
            }
        }

        public ICommand HomeCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand TrophyCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand RulesCommand { get; }
        public ICommand PlusCommand { get; }
        public ICommand MinusCommand { get; }
        public ICommand SaveCommand { get; }
        private ProgramManager ProgramManager;

        public VolumeViewModel()
        {
            HomeCommand = new RelayCommand(param => OpenHomeWindow(param));
            ExitCommand = new RelayCommand(param => OpenExitWindow(param));
            TrophyCommand = new RelayCommand(param => OpenTrophyWindow(param));
            SettingsCommand = new RelayCommand(param => OpenSettingsWindow(param));
            RulesCommand = new RelayCommand(param => OpenRulesWindow(param));
            PlusCommand = new RelayCommand(param => IncreaseVolume());
            MinusCommand = new RelayCommand(param => DecreaseVolume());
            SaveCommand = new RelayCommand(param => SaveSettings(param));
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

        private void OpenRulesWindow(object? parameter)
        {
            var rulesWindow = new RulesWindow();
            rulesWindow.Show();
            CloseCurrentWindow(parameter);
        }
        private void IncreaseVolume()
        {
            SendMessageW(new WindowInteropHelper(Application.Current.MainWindow).Handle, WM_APPCOMMAND, new IntPtr(0), new IntPtr(APPCOMMAND_VOLUME_UP));
            CurrentVolume = Math.Min(CurrentVolume + 2, 100);
        }

        private void DecreaseVolume()
        {
            SendMessageW(new WindowInteropHelper(Application.Current.MainWindow).Handle, WM_APPCOMMAND, new IntPtr(0), new IntPtr(APPCOMMAND_VOLUME_DOWN));
            CurrentVolume = Math.Max(CurrentVolume - 2, 0);
        }

        private void SaveSettings(object? parameter)
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
