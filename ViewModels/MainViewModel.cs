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

    }
}
