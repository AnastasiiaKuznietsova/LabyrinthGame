using System.Windows;

namespace LabyrinthGame
{
    public partial class ChangeProfileWindow : Window
    {
        public ChangeProfileWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = new ChangeProfileViewModel();
        }

    }
}