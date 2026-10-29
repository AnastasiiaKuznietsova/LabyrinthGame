using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class DrawWindow : Window
    {
        public DrawWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new DrawViewModel();

        }
    }
}