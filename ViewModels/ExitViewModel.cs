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

    }
}
