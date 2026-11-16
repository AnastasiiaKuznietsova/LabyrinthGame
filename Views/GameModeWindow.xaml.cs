using System.Windows;

namespace LabyrinthGame
{
    public partial class GameModeWindow : Window
    {
        public GameModeWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new GameModeViewModel();
        }
    }
}