using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class MultiPlayerLocallyWindow : Window
    {
        public MultiPlayerLocallyWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new MultiPlayerLocallyViewModel();
        }
    }
}