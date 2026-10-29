using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class SinglePlayerWindow : Window
    {
        public SinglePlayerWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new SinglePlayerViewModel();
        }
    }
}