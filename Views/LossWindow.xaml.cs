using LabyrinthGame.ViewModels;
using System.Windows;

namespace LabyrinthGame
{
    public partial class VolumeWindow : Window
    {
        public partial class LossWindow : Window
        {
            public LossWindow()
            {
                InitializeComponent();
                this.WindowState = WindowState.Maximized;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.DataContext = new LossViewModel();

            }
        }
    }
}