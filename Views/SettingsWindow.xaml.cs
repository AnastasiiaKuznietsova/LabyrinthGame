using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new SettingsViewModel();
        }
    }
}